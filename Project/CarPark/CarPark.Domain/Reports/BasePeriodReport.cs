using CarPark.DateTimes;
using CarPark.Reports.Abstract;

namespace CarPark.Reports;

public abstract class BasePeriodReport<TData> : BaseReport<IReadOnlyList<DataPeriodItem<TData>>>
{
    public PeriodType Period { get; private init; }

    public UtcDateTimeOffset StartDate { get; private init; }

    public UtcDateTimeOffset EndDate { get; private init; }

    protected BasePeriodReport(string reportName, 
        PeriodType period, 
        UtcDateTimeOffset startDate, 
        UtcDateTimeOffset endDate, 
        IEnumerable<DataPeriodItem<TData>> dataItems) 
        : base(reportName, dataItems.ToList())
    {
        Period = period;

        StartDate = startDate;
        EndDate = endDate;
    }
}