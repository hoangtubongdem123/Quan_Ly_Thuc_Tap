public class ThucTapSubject : ISubject
{
    private List<IObserver> _observers =
        new List<IObserver>();

    public void Attach(IObserver observer)
    {
        _observers.Add(observer);
    }

    public async Task Notify(string ma_nguoi_nhan,string tieu_de,string noi_dung)
    {
        foreach (var observer in _observers)
        {
            await observer.Update(ma_nguoi_nhan,tieu_de,noi_dung);
        }
    }
}