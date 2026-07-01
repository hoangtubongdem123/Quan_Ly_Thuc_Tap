import { useEffect, useMemo, useState } from 'react';
import { logout } from '../../services/authService';
import {
  chamDiemSinhVien,
  getKetQuaDanhGiaGiangVien,
  getMinhChungByMssv,
  getMySinhVienPhuTrach,
  getTieuChiDanhGia,
} from '../../services/giangVienService';
import { API_BASE_URL } from '../../services/api';
import './GiangVienDashboard.css';

function formatDate(value) {
  return value ? new Intl.DateTimeFormat('vi-VN').format(new Date(value)) : '-';
}

function formatScore(value) {
  return value === null || value === undefined ? '-' : Number(value).toFixed(1);
}

function getMssv(student) {
  return student?.mssv ?? student?.mSSV ?? student?.MSSV ?? '';
}

function getMinhChungUrl(path) {
  if (!path) return '#';
  return path.startsWith('http') ? path : `${API_BASE_URL}${path}`;
}

function GiangVienDashboard({ account, onLogout }) {
  const [students, setStudents] = useState([]);
  const [selectedStudentKey, setSelectedStudentKey] = useState('');
  const [criteria, setCriteria] = useState(null);
  const [scoreResult, setScoreResult] = useState(null);
  const [minhChungList, setMinhChungList] = useState([]);
  const [minhChungLoading, setMinhChungLoading] = useState(false);
  const [minhChungModalOpen, setMinhChungModalOpen] = useState(false);
  const [scores, setScores] = useState({});
  const [chang, setChang] = useState(1);
  const [studentSearch, setStudentSearch] = useState('');
  const [loading, setLoading] = useState(true);
  const [criteriaLoading, setCriteriaLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [notice, setNotice] = useState(null);

  const selectedStudent = useMemo(
    () => students.find((student) => `${student.idKiThucTap}-${getMssv(student)}` === selectedStudentKey) ?? students[0] ?? null,
    [students, selectedStudentKey]
  );

  const filteredStudents = useMemo(() => {
    const keyword = studentSearch.trim().toLowerCase();
    if (!keyword) return students;

    return students.filter((student) =>
      `${getMssv(student)} ${student.tenSinhVien} ${student.tenKiThucTap}`.toLowerCase().includes(keyword)
    );
  }, [students, studentSearch]);

  useEffect(() => {
    let mounted = true;

    async function loadStudents() {
      setLoading(true);
      setNotice(null);

      try {
        const result = await getMySinhVienPhuTrach();
        if (!mounted) return;

        const list = Array.isArray(result) ? result : [];
        setStudents(list);
        setSelectedStudentKey(list[0] ? `${list[0].idKiThucTap}-${getMssv(list[0])}` : '');
      } catch (error) {
        if (mounted) {
          setStudents([]);
          setNotice({ type: 'error', text: error.message });
        }
      } finally {
        if (mounted) setLoading(false);
      }
    }

    loadStudents();

    return () => {
      mounted = false;
    };
  }, []);

  useEffect(() => {
    let mounted = true;

    async function loadCriteriaAndScores() {
      if (!selectedStudent?.idKiThucTap || !getMssv(selectedStudent)) {
        setCriteria(null);
        setScoreResult(null);
        setScores({});
        return;
      }

      setCriteriaLoading(true);
      setNotice(null);

      try {
        const [criteriaResult, result] = await Promise.all([
          getTieuChiDanhGia(selectedStudent.idKiThucTap),
          getKetQuaDanhGiaGiangVien(selectedStudent.idKiThucTap, getMssv(selectedStudent)),
        ]);

        if (!mounted) return;

        setCriteria(criteriaResult);
        setScoreResult(result);
      } catch (error) {
        if (mounted) {
          setCriteria(null);
          setScoreResult(null);
          setNotice({ type: 'error', text: error.message });
        }
      } finally {
        if (mounted) setCriteriaLoading(false);
      }
    }

    loadCriteriaAndScores();

    return () => {
      mounted = false;
    };
  }, [selectedStudent]);

  useEffect(() => {
    let mounted = true;

    async function loadMinhChung() {
      if (!getMssv(selectedStudent)) {
        setMinhChungList([]);
        return;
      }

      setMinhChungLoading(true);

      try {
        const result = await getMinhChungByMssv(getMssv(selectedStudent));
        if (mounted) setMinhChungList(Array.isArray(result) ? result : []);
      } catch {
        if (mounted) setMinhChungList([]);
      } finally {
        if (mounted) setMinhChungLoading(false);
      }
    }

    loadMinhChung();

    return () => {
      mounted = false;
    };
  }, [selectedStudent]);

  useEffect(() => {
    const nextScores = {};

    for (const clo of criteria?.clos || []) {
      const resultClo = (scoreResult?.ketQuaClo || []).find((item) =>
        String(item.maClo).toLowerCase() === String(clo.tenClo).toLowerCase()
      );

      const value = chang === 1
        ? resultClo?.diemChang1Gvhd
        : resultClo?.diemChang2Gvhd;

      nextScores[clo.tenClo] = value === null || value === undefined ? '' : String(value);
    }

    setScores(nextScores);
  }, [criteria, scoreResult, chang]);

  async function handleLogout() {
    try {
      await logout();
    } catch {
      // Local logout remains useful when the server session already ended.
    } finally {
      onLogout?.();
    }
  }

  function updateScore(maClo, value) {
    setScores((current) => ({
      ...current,
      [maClo]: value,
    }));
  }

  async function handleSubmitScores(event) {
    event.preventDefault();

    if (!selectedStudent || !criteria?.clos?.length) {
      setNotice({ type: 'error', text: 'Chưa chọn sinh viên hoặc chưa có tiêu chí.' });
      return;
    }

    const diemClo = criteria.clos.map((clo) => ({
      maClo: clo.tenClo,
      diem: Number(scores[clo.tenClo]),
    }));

    if (diemClo.some((item) => Number.isNaN(item.diem) || item.diem < 0 || item.diem > 10)) {
      setNotice({ type: 'error', text: 'Điểm CLO phải nằm trong khoảng 0 đến 10.' });
      return;
    }

    setSaving(true);
    setNotice(null);

    try {
      await chamDiemSinhVien({
        mssv: getMssv(selectedStudent),
        idKiThucTap: selectedStudent.idKiThucTap,
        chang,
        nguoiChamLoai: 'GVHD',
        nguoiChamId: account?.maSoGiangVien,
        diemClo,
      });

      const result = await getKetQuaDanhGiaGiangVien(
        selectedStudent.idKiThucTap,
        getMssv(selectedStudent)
      );

      setScoreResult(result);
      setNotice({ type: 'success', text: 'Đã lưu điểm thực tập.' });
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setSaving(false);
    }
  }

  return (
    <main className="gv-dashboard">
      <aside className="gv-sidebar">
        <div className="gv-brand">
          <div className="gv-mark">GV</div>
          <div>
            <span>Giảng viên hướng dẫn</span>
            <strong>{account?.tenGiangVien || account?.email || 'Giảng viên'}</strong>
          </div>
        </div>

        <div className="gv-meta">
          <span>Mã giảng viên</span>
          <strong>{account?.maSoGiangVien || '-'}</strong>
          <span>Sinh viên phụ trách</span>
          <strong>{students.length}</strong>
        </div>

        <button className="gv-logout" type="button" onClick={handleLogout}>
          Đăng xuất
        </button>
      </aside>

      <section className="gv-main">
        <header className="gv-toolbar">
          <div>
            <span>Chấm điểm thực tập</span>
            <h1>Sinh viên được phân công</h1>
          </div>
          <div className="gv-chang-switch" role="group" aria-label="Chọn chặng chấm điểm">
            <button type="button" className={chang === 1 ? 'active' : ''} onClick={() => setChang(1)}>
              Chặng 1
            </button>
            <button type="button" className={chang === 2 ? 'active' : ''} onClick={() => setChang(2)}>
              Chặng 2
            </button>
          </div>
        </header>

        {notice && (
          <div className={`gv-notice ${notice.type}`} role="status">
            {notice.text}
          </div>
        )}

        <section className="gv-workspace">
          <aside className="gv-student-panel">
            <input
              value={studentSearch}
              onChange={(event) => setStudentSearch(event.target.value)}
              placeholder="Tìm MSSV, tên sinh viên, kỳ"
            />

            <div className="gv-student-list">
              {loading && <div className="gv-empty">Đang tải sinh viên...</div>}

              {!loading && filteredStudents.map((student) => {
                const key = `${student.idKiThucTap}-${getMssv(student)}`;

                return (
                  <button
                    key={key}
                    type="button"
                    className={selectedStudentKey === key ? 'active' : ''}
                    onClick={() => setSelectedStudentKey(key)}
                  >
                    <strong>{student.tenSinhVien || '-'}</strong>
                    <span>{getMssv(student)} · {student.tenKiThucTap}</span>
                    <small>{student.tenDonViHD || 'Chưa có ĐVHD'}</small>
                  </button>
                );
              })}

              {!loading && filteredStudents.length === 0 && (
                <div className="gv-empty">Chưa có sinh viên được phân công.</div>
              )}
            </div>
          </aside>

          <section className="gv-score-panel">
            <div className="gv-summary">
              <div>
                <span>Sinh viên</span>
                <strong>{selectedStudent?.tenSinhVien || '-'}</strong>
              </div>
              <div>
                <span>MSSV</span>
                <strong>{getMssv(selectedStudent) || '-'}</strong>
              </div>
              <div>
                <span>Kỳ thực tập</span>
                <strong>{selectedStudent?.tenKiThucTap || '-'}</strong>
              </div>
              <div>
                <span>Điểm học phần</span>
                <strong>{formatScore(scoreResult?.diemHocPhan)}</strong>
              </div>
              <div>
                <span>Minh chứng</span>
                <strong>{minhChungLoading ? 'Đang tải...' : `${minhChungList.length} file`}</strong>
              </div>
            </div>

            <form className="gv-score-form" onSubmit={handleSubmitScores}>
              <div className="gv-score-table-wrap">
                <table className="gv-score-table">
                  <thead>
                    <tr>
                      <th>CLO</th>
                      <th>Mô tả</th>
                      <th>Trọng số GVHD</th>
                      <th>Điểm GVHD chặng {chang}</th>
                      <th>Điểm CLO hiện tại</th>
                    </tr>
                  </thead>
                  <tbody>
                    {criteriaLoading && (
                      <tr>
                        <td colSpan="5">Đang tải tiêu chí...</td>
                      </tr>
                    )}

                    {!criteriaLoading && (criteria?.clos || []).map((clo) => {
                      const resultClo = (scoreResult?.ketQuaClo || []).find((item) =>
                        String(item.maClo).toLowerCase() === String(clo.tenClo).toLowerCase()
                      );

                      return (
                        <tr key={clo.tenClo}>
                          <td>{clo.tenClo}</td>
                          <td>{clo.moTaClo || '-'}</td>
                          <td>{Number(clo.trongSoGiangVienHuongDan || 0)}%</td>
                          <td>
                            <input
                              type="number"
                              min="0"
                              max="10"
                              step="0.1"
                              value={scores[clo.tenClo] ?? ''}
                              onChange={(event) => updateScore(clo.tenClo, event.target.value)}
                              required
                            />
                          </td>
                          <td>{formatScore(resultClo?.diemClo)}</td>
                        </tr>
                      );
                    })}

                    {!criteriaLoading && (!criteria?.clos || criteria.clos.length === 0) && (
                      <tr>
                        <td colSpan="5">Kỳ này chưa có tiêu chí CLO.</td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>

              <div className="gv-score-footer">
              <div>
                <span>Thời gian kỳ</span>
                <strong>{formatDate(selectedStudent?.timeBatDau)} - {formatDate(selectedStudent?.timeKetThuc)}</strong>
              </div>
              <div className="gv-score-actions">
                <button type="button" className="secondary" onClick={() => setMinhChungModalOpen(true)} disabled={!getMssv(selectedStudent)}>
                  Xem minh chứng
                </button>
                <button type="submit" disabled={saving || criteriaLoading || !criteria?.clos?.length}>
                  {saving ? 'Đang lưu...' : `Lưu điểm chặng ${chang}`}
                </button>
              </div>
            </div>
            </form>
          </section>
        </section>
      </section>

      {minhChungModalOpen && (
        <div className="gv-modal-backdrop" role="presentation" onMouseDown={() => setMinhChungModalOpen(false)}>
          <section className="gv-modal" onMouseDown={(event) => event.stopPropagation()}>
            <div className="gv-modal-header">
              <span>Minh chứng sinh viên</span>
              <button type="button" aria-label="Đóng" onClick={() => setMinhChungModalOpen(false)}>x</button>
            </div>

            <div className="gv-modal-summary">
              <div>
                <span>Sinh viên</span>
                <strong>{selectedStudent?.tenSinhVien || '-'}</strong>
              </div>
              <div>
                <span>MSSV</span>
                <strong>{getMssv(selectedStudent) || '-'}</strong>
              </div>
              <div>
                <span>Số minh chứng</span>
                <strong>{minhChungList.length}</strong>
              </div>
            </div>

            <div className="gv-evidence-list">
              {minhChungLoading && <div className="gv-empty">Đang tải minh chứng...</div>}

              {!minhChungLoading && minhChungList.map((item) => (
                <article key={item.idMinhChung}>
                  <div>
                    <strong>{item.tenMinhChung}</strong>
                    <span>{item.path?.split('/').pop() || item.path}</span>
                  </div>
                  <a href={getMinhChungUrl(item.path)} target="_blank" rel="noreferrer">
                    Xem file
                  </a>
                </article>
              ))}

              {!minhChungLoading && minhChungList.length === 0 && (
                <div className="gv-empty">Sinh viên này chưa nộp minh chứng.</div>
              )}
            </div>
          </section>
        </div>
      )}
    </main>
  );
}

export default GiangVienDashboard;
