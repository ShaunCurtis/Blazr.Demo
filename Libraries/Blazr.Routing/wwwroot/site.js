window.blazr_setEditorExitCheck = function (dotNetHelper, show) {
    if (show) {
        window.addEventListener("beforeunload", blazr_showExitDialog);
        blazrDotNetExitHelper = dotNetHelper;
    }
    else {
        window.removeEventListener("beforeunload", blazr_showExitDialog);
        blazrDotNetExitHelper = null;
    }
}

var blazrDotNetExitHelper;

window.blazr_showExitDialog = function (event) {
    event.preventDefault();
    event.returnValue = "There are unsaved changes on this page.  Do you want to leave?";
    blazrDotNetExitHelper.invokeMethodAsync("AgentExitAttempt");
}
