import { useEffect, useMemo, useState } from 'react';
import { logout } from '../../services/authService';
import {
  addMinhChung,
  deleteMinhChung,
  getCurrentSinhVien,
  getKetQuaDanhGiaSinhVien,
  getMinhChungByMssv,
  getMyKiThucTap,
  getMyThongBao,
  updateMinhChung,
} from '../../services/sinhVienService';
import { API_BASE_URL } from '../../services/api';
import './SinhVienDashboard.css';

function formatDate(value) {
  return value ? new Intl.DateTimeFormat('vi-VN').format(new Date(value)) : '-';
}

function formatScore(value) {
  return value === null || value === undefined ? '-' : Number(value).toFixed(1);
}

function normalizeMssv(student, selectedPeriod) {
  return String(
    student?.mssv
      ?? student?.Mssv
      ?? selectedPeriod?.mssv
      ?? selectedPeriod?.mSSV
      ?? selectedPeriod?.MSSV
      ?? ''
  );
}

function getMinhChungUrl(path) {
  if (!path) return '#';
  return path.startsWith('http') ? path : `${API_BASE_URL}${path}`;
}

function SinhVienDashboard({ account, onLogout }) {
  const [student, setStudent] = useState(null);
  const [periods, setPeriods] = useState([]);
  const [notifications, setNotifications] = useState([]);
  const [selectedPeriodId, setSelectedPeriodId] = useState(null);
  const [scoreResult, setScoreResult] = useState(null);
  const [minhChungList, setMinhChungList] = useState([]);
  const [minhChungForm, setMinhChungForm] = useState({
    tenMinhChung: '',
    file: null,
  });
  const [editingMinhChung, setEditingMinhChung] = useState(null);
  const [loading, setLoading] = useState(true);
  const [scoreLoading, setScoreLoading] = useState(false);
  const [minhChungLoading, setMinhChungLoading] = useState(false);
  const [savingMinhChung, setSavingMinhChung] = useState(false);
  const [deletingMinhChungId, setDeletingMinhChungId] = useState(null);
  const [fileInputKey, setFileInputKey] = useState(0);
  const [notice, setNotice] = useState(null);

  const selectedPeriod = useMemo(
    () => periods.find((period) => period.idKiThucTap === selectedPeriodId) ?? periods[0] ?? null,
    [periods, selectedPeriodId]
  );

  const mssv = normalizeMssv(student, selectedPeriod);

  useEffect(() => {
    let mounted = true;

    async function loadDashboard() {
      setLoading(true);
      setNotice(null);

      try {
        const [studentInfo, periodList, notificationList] = await Promise.all([
          getCurrentSinhVien(),
          getMyKiThucTap(),
          getMyThongBao(),
        ]);

        if (!mounted) return;

        setStudent(studentInfo);
        setPeriods(Array.isArray(periodList) ? periodList : []);
        setNotifications(Array.isArray(notificationList) ? notificationList : []);
        setSelectedPeriodId(periodList?.[0]?.idKiThucTap ?? null);
      } catch (error) {
        if (mounted) {
          setNotice({ type: 'error', text: error.message });
        }
      } finally {
        if (mounted) setLoading(false);
      }
    }

    loadDashboard();

    return () => {
      mounted = false;
    };
  }, []);

  useEffect(() => {
    let mounted = true;

    async function loadScores() {
      if (!selectedPeriod?.idKiThucTap || !mssv) {
        setScoreResult(null);
        return;
      }

      setScoreLoading(true);

      try {
        const result = await getKetQuaDanhGiaSinhVien(selectedPeriod.idKiThucTap, mssv);
        if (mounted) setScoreResult(result);
      } catch (error) {
        if (mounted) {
          setScoreResult(null);
          setNotice({ type: 'error', text: error.message });
        }
      } finally {
        if (mounted) setScoreLoading(false);
      }
    }

    loadScores();

    return () => {
      mounted = false;
    };
  }, [selectedPeriod?.idKiThucTap, mssv]);

  useEffect(() => {
    let mounted = true;

    async function loadMinhChung() {
      if (!mssv) {
        setMinhChungList([]);
        return;
      }

      setMinhChungLoading(true);

      try {
        const result = await getMinhChungByMssv(mssv);
        if (mounted) setMinhChungList(Array.isArray(result) ? result : []);
      } catch (error) {
        if (mounted) {
          setMinhChungList([]);
          setNotice({ type: 'error', text: error.message });
        }
      } finally {
        if (mounted) setMinhChungLoading(false);
      }
    }

    loadMinhChung();

    return () => {
      mounted = false;
    };
  }, [mssv]);

  function resetMinhChungForm() {
    setMinhChungForm({
      tenMinhChung: '',
      file: null,
    });
    setEditingMinhChung(null);
    setFileInputKey((current) => current + 1);
  }

  async function reloadMinhChung() {
    if (!mssv) return;

    const result = await getMinhChungByMssv(mssv);
    setMinhChungList(Array.isArray(result) ? result : []);
  }

  function handleEditMinhChung(item) {
    setEditingMinhChung(item);
    setMinhChungForm({
      tenMinhChung: item.tenMinhChung || '',
      file: null,
    });
    setFileInputKey((current) => current + 1);
  }

  async function handleSubmitMinhChung(event) {
    event.preventDefault();

    if (!mssv) {
      setNotice({ type: 'error', text: 'Thiếu MSSV.' });
      return;
    }

    if (!minhChungForm.tenMinhChung.trim()) {
      setNotice({ type: 'error', text: 'Tên minh chứng không được để trống.' });
      return;
    }

    if (!editingMinhChung && !minhChungForm.file) {
      setNotice({ type: 'error', text: 'Vui lòng chọn file minh chứng.' });
      return;
    }

    setSavingMinhChung(true);
    setNotice(null);

    try {
      if (editingMinhChung) {
        await updateMinhChung(editingMinhChung.idMinhChung, minhChungForm);
        setNotice({ type: 'success', text: 'Đã cập nhật minh chứng.' });
      } else {
        await addMinhChung({
          ...minhChungForm,
          mssv,
        });
        setNotice({ type: 'success', text: 'Đã thêm minh chứng.' });
      }

      resetMinhChungForm();
      await reloadMinhChung();
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setSavingMinhChung(false);
    }
  }

  async function handleDeleteMinhChung(item) {
    const ok = window.confirm(`Xóa minh chứng "${item.tenMinhChung}"?`);
    if (!ok) return;

    setDeletingMinhChungId(item.idMinhChung);
    setNotice(null);

    try {
      await deleteMinhChung(item.idMinhChung);
      setNotice({ type: 'success', text: 'Đã xóa minh chứng.' });

      if (editingMinhChung?.idMinhChung === item.idMinhChung) {
        resetMinhChungForm();
      }

      await reloadMinhChung();
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setDeletingMinhChungId(null);
    }
  }

  async function handleLogout() {
    try {
      await logout();
    } catch {
      // Local logout still clears the screen when the server session already ended.
    } finally {
      onLogout?.();
    }
  }

  return (
    <main className="sv-dashboard">
      <aside className="sv-sidebar">
        <div className="sv-brand">
          <div className="sv-mark">SV</div>
          <div>
            <span>Sinh viên</span>
            <strong>{student?.tenSinhVien || account?.email || 'Đang tải...'}</strong>
          </div>
        </div>

        <div className="sv-student-meta">
          <span>MSSV</span>
          <strong>{mssv || '-'}</strong>
          <span>Email</span>
          <strong>{student?.gmailSinhVien || account?.email || '-'}</strong>
        </div>

        <button className="sv-logout" type="button" onClick={handleLogout}>
          Đăng xuất
        </button>
      </aside>

      <section className="sv-main">
        <header className="sv-toolbar">
          <div>
            <span>Thực tập tốt nghiệp</span>
            <h1>Theo dõi kỳ thực tập và điểm CLO</h1>
          </div>
          <div className="sv-summary">
            <div>
              <span>Kỳ thực tập</span>
              <strong>{periods.length}</strong>
            </div>
            <div>
              <span>Thông báo</span>
              <strong>{notifications.length}</strong>
            </div>
          </div>
        </header>

        {notice && (
          <div className={`sv-notice ${notice.type}`} role="status">
            {notice.text}
          </div>
        )}

        <section className="sv-grid">
          <div className="sv-panel sv-periods">
            <div className="sv-panel-head">
              <h2>Kỳ thực tập</h2>
              <span>{loading ? 'Đang tải...' : `${periods.length} kỳ`}</span>
            </div>

            <div className="sv-period-list">
              {!loading && periods.map((period) => (
                <button
                  type="button"
                  key={period.idKiThucTap}
                  className={selectedPeriod?.idKiThucTap === period.idKiThucTap ? 'active' : ''}
                  onClick={() => setSelectedPeriodId(period.idKiThucTap)}
                >
                  <strong>{period.tenKiThucTap}</strong>
                  <span>{formatDate(period.timeBatDau)} - {formatDate(period.timeKetThuc)}</span>
                  <small>{period.trangThaiPhanCong || period.trangThaiKiThucTap}</small>
                </button>
              ))}

              {!loading && periods.length === 0 && (
                <div className="sv-empty">Bạn chưa được thêm vào kỳ thực tập nào.</div>
              )}
            </div>
          </div>

          <div className="sv-panel sv-assignment">
            <div className="sv-panel-head">
              <h2>Phân công</h2>
              <span>{selectedPeriod?.trangThaiPhanCong || '-'}</span>
            </div>

            <div className="sv-assignment-body">
              <div>
                <span>Kỳ</span>
                <strong>{selectedPeriod?.tenKiThucTap || '-'}</strong>
              </div>
              <div>
                <span>Đơn vị hướng dẫn</span>
                <strong>{selectedPeriod?.tenDonViHD || 'Chưa có'}</strong>
              </div>
              <div>
                <span>Giảng viên hướng dẫn</span>
                <strong>{selectedPeriod?.tenGiangVien || 'Chưa phân công'}</strong>
              </div>
            </div>
          </div>

          <div className="sv-panel sv-evidence">
            <div className="sv-panel-head">
              <h2>Minh chứng</h2>
              <span>{minhChungLoading ? 'Đang tải...' : `${minhChungList.length} file`}</span>
            </div>

            <form className="sv-evidence-form" onSubmit={handleSubmitMinhChung}>
              <label>
                <span>Tên minh chứng</span>
                <input
                  value={minhChungForm.tenMinhChung}
                  onChange={(event) => setMinhChungForm((current) => ({
                    ...current,
                    tenMinhChung: event.target.value,
                  }))}
                  placeholder="VD: Báo cáo thực tập"
                />
              </label>

              <label>
                <span>{editingMinhChung ? 'File mới nếu cần thay' : 'File minh chứng'}</span>
                <input
                  key={fileInputKey}
                  type="file"
                  onChange={(event) => setMinhChungForm((current) => ({
                    ...current,
                    file: event.target.files?.[0] ?? null,
                  }))}
                />
              </label>

              <div className="sv-evidence-actions">
                <button type="submit" disabled={savingMinhChung}>
                  {savingMinhChung
                    ? 'Đang lưu...'
                    : editingMinhChung
                      ? 'Cập nhật'
                      : 'Thêm minh chứng'}
                </button>
                {editingMinhChung && (
                  <button type="button" className="secondary" onClick={resetMinhChungForm}>
                    Hủy sửa
                  </button>
                )}
              </div>
            </form>

            <div className="sv-evidence-list">
              {minhChungList.map((item) => (
                <article key={item.idMinhChung}>
                  <div>
                    <strong>{item.tenMinhChung}</strong>
                    <span>{item.path?.split('/').pop() || item.path}</span>
                  </div>
                  <div className="sv-evidence-row-actions">
                    <a href={getMinhChungUrl(item.path)} target="_blank" rel="noreferrer">
                      Xem
                    </a>
                    <button type="button" onClick={() => handleEditMinhChung(item)}>
                      Sửa
                    </button>
                    <button
                      type="button"
                      className="danger"
                      disabled={deletingMinhChungId === item.idMinhChung}
                      onClick={() => handleDeleteMinhChung(item)}
                    >
                      {deletingMinhChungId === item.idMinhChung ? 'Đang xóa...' : 'Xóa'}
                    </button>
                  </div>
                </article>
              ))}

              {!minhChungLoading && minhChungList.length === 0 && (
                <div className="sv-empty">Chưa có minh chứng.</div>
              )}
            </div>
          </div>

          <div className="sv-panel sv-scores">
            <div className="sv-panel-head">
              <h2>Điểm CLO</h2>
              <span>{scoreLoading ? 'Đang tải...' : scoreResult?.diemHocPhan ? `HP ${formatScore(scoreResult.diemHocPhan)}` : 'Chưa đủ điểm'}</span>
            </div>

            <div className="sv-score-table-wrap">
              <table className="sv-score-table">
                <thead>
                  <tr>
                    <th>CLO</th>
                    <th>Chặng 1 ĐVHD</th>
                    <th>Chặng 1 GVHD</th>
                    <th>Điểm C1</th>
                    <th>Chặng 2 ĐVHD</th>
                    <th>Chặng 2 GVHD</th>
                    <th>Điểm C2</th>
                    <th>Điểm CLO</th>
                    <th>Mức đạt</th>
                  </tr>
                </thead>
                <tbody>
                  {scoreLoading && (
                    <tr>
                      <td colSpan="9">Đang tải điểm...</td>
                    </tr>
                  )}

                  {!scoreLoading && (scoreResult?.ketQuaClo || []).map((clo) => (
                    <tr key={clo.maClo}>
                      <td>
                        <strong>{clo.maClo}</strong>
                        <span>{clo.moTaClo || clo.tenClo}</span>
                      </td>
                      <td>{formatScore(clo.diemChang1Dvhd)}</td>
                      <td>{formatScore(clo.diemChang1Gvhd)}</td>
                      <td>{formatScore(clo.diemChang1)}</td>
                      <td>{formatScore(clo.diemChang2Dvhd)}</td>
                      <td>{formatScore(clo.diemChang2Gvhd)}</td>
                      <td>{formatScore(clo.diemChang2)}</td>
                      <td>{formatScore(clo.diemClo)}</td>
                      <td>{clo.mucDat ?? '-'}</td>
                    </tr>
                  ))}

                  {!scoreLoading && (!scoreResult?.ketQuaClo || scoreResult.ketQuaClo.length === 0) && (
                    <tr>
                      <td colSpan="9">Chưa có dữ liệu điểm cho kỳ này.</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>

            <div className="sv-score-footer">
              <div>
                <span>Người chấm</span>
                <strong>ĐVHD: {selectedPeriod?.tenDonViHD || '-'} | GVHD: {selectedPeriod?.tenGiangVien || '-'}</strong>
              </div>
              <div>
                <span>Kết quả học phần</span>
                <strong>{scoreResult?.duDieuKienHoanThanh ? 'Đủ điều kiện hoàn thành' : 'Chưa đủ dữ liệu/điều kiện'}</strong>
              </div>
            </div>
          </div>

          <div className="sv-panel sv-notifications">
            <div className="sv-panel-head">
              <h2>Thông báo</h2>
              <span>{notifications.length}</span>
            </div>

            <div className="sv-notification-list">
              {notifications.map((item) => (
                <article key={item.idThongBao}>
                  <time>{formatDate(item.ngayTao)}</time>
                  <strong>{item.tieuDe}</strong>
                  <p>{item.noiDung}</p>
                </article>
              ))}

              {!loading && notifications.length === 0 && (
                <div className="sv-empty">Chưa có thông báo phân công.</div>
              )}
            </div>
          </div>
        </section>
      </section>
    </main>
  );
}

export default SinhVienDashboard;
