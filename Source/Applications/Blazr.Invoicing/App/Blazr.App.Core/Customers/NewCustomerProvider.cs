﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class NewCustomerProvider : INewRecordProvider<DmoCustomer>
{
    public DmoCustomer NewRecord()
    {
        return new DmoCustomer() { CustomerId = new(Guid.NewGuid()) };
    }
}