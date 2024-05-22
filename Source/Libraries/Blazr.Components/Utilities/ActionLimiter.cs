/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Components;

public sealed class ActionLimiter
{
    private int _backOffPeriod = 0;
    private Func<Task> _taskToRun;
    private Task _activeTask = Task.CompletedTask;
    private TaskCompletionSource<bool>? _queuedTaskCompletionSource;
    private TaskCompletionSource<bool>? _activeTaskCompletionSource;

    private async Task RunQueueAsync()
    {
        // if we have a completed task then null it
        if (_activeTaskCompletionSource is not null && _activeTaskCompletionSource.Task.IsCompleted)
            _activeTaskCompletionSource = null;

        // if we have a running task then everything is already in motion and there's nothing to do
        if (_activeTaskCompletionSource is not null)
            return;

        // run the loop while we have a queued request.
        while (_queuedTaskCompletionSource is not null)
        {
            // assign the queued task reference to the running task  
            _activeTaskCompletionSource = _queuedTaskCompletionSource;
            // And release the reference
            _queuedTaskCompletionSource = null;

            // start backoff task
            var backoffTask = Task.Delay(_backOffPeriod);

            // start main task
            var mainTask = _taskToRun.Invoke();

            // await both ensures we run the backoff period or greater
            await Task.WhenAll( new Task[] { mainTask, backoffTask } );

            // Set the running task completion as complete
            _activeTaskCompletionSource.TrySetResult(true);

            // and release our reference to the running task completion
            // The originator will still hold a reference and can act on it's completion
            _activeTaskCompletionSource = null;


            // back to the top to check if another task has been queued
        }

        return;
    }

    public Task<bool> QueueAsync()
    {
        var oldCompletionTask = _queuedTaskCompletionSource;

        // Create a new completion task
        var newCompletionTask = new TaskCompletionSource<bool>();

        // get the actual task before we assign it to the queue
        var task = newCompletionTask.Task;

        // replace _queuedTaskCompletionSource
        _queuedTaskCompletionSource = newCompletionTask;

        // check if we already have a queued queued task.
        // If so set it as completed, false = not run 
        if (oldCompletionTask is not null && !oldCompletionTask.Task.IsCompleted)
            oldCompletionTask?.TrySetResult(false);

        // if we don't have a running task or the task is complete , then there's no process running the queue
        // So we need to call it and assign it to `runningTask`
        if (_activeTask is null || _activeTask.IsCompleted)
            _activeTask = this.RunQueueAsync();

        // return the reference to the task we queued
        return task;
    }

    private ActionLimiter(Func<Task> toRun, int backOffPeriod)
    {
        _backOffPeriod = backOffPeriod;
        _taskToRun = toRun;
    }

    /// <summary>
    /// Static method to create a new deBouncer
    /// </summary>
    /// <param name="toRun">method to run to update the component</param>
    /// <param name="backOffPeriod">Back off period in millisecs</param>
    /// <returns></returns>
    public static ActionLimiter Create(Func<Task> toRun, int backOffPeriod)
            => new ActionLimiter(toRun, backOffPeriod > 300 ? backOffPeriod : 300);
}
