namespace CarPark.Reports.Abstract;

public abstract class BaseReport<TData>
{
    public string ReportName { get; private init; }

    public TData DataItems { get; private init; }

    protected BaseReport(string reportName, TData data)
    {
        ReportName = reportName;

        DataItems = data;
    }
}