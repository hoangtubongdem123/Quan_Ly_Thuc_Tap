public interface ISubject
{
    void Attach(IObserver observer);

    Task Notify(string ma_nguoi_nhan,string tieu_de,string noi_dung);
}