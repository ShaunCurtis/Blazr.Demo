/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class InvoicesByMonthSpecification : PredicateSpecification<Invoice>
{
    private int _month;
    private int _year;

    public InvoicesByMonthSpecification(int month, int year)
    {
        _month = month;
        _year = year;
    }

    public InvoicesByMonthSpecification(FilterDefinition filter)
    {
        if (filter.TryFromJson<Tuple<int, int>>(out Tuple<int, int>? value))
        {
            _month = value.Item1;
            _year = value.Item2;
        }
    }

    public override Expression<Func<Invoice, bool>> Expression
        => invoice => invoice.InvoiceDate.Month == _month && invoice.InvoiceDate.Year == _year;
}
