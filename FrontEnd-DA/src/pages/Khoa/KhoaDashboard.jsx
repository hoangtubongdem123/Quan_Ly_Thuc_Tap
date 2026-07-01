import { useEffect, useMemo, useRef, useState } from 'react';
import { logout } from '../../services/authService';
import { API_BASE_URL } from '../../services/api';
import {
  capNhatDiemThucTapTuKhoa,
  createKiThucTap,
  createDonViHuongDan,
  createTieuChi,
  deleteDonViHuongDan,
  deleteKiThucTap,
  deleteSinhVienThucTap,
  deleteTieuChi,
  getCurrentKhoa,
  getAllGiangVien,
  getAllSinhVien,
  getGiangVienPhuTrachCounts,
  getKetQuaDanhGia,
  getKiThucTapByKhoa,
  getMinhChungByMssv,
  getDonViHuongDanByKi,
  getSinhVienThucTapByKi,
  getTieuChiByKhoa,
  importSinhVienThucTap,
  phanCongGVHD,
  previewImportSinhVienThucTap,
  updateDonViHuongDan,
  updateKiThucTap,
  updateTieuChi,
} from '../../services/khoaService';
import './KhoaDashboard.css';

const emptyClo = { tenClo: '', moTaClo: '', trongSoHp: 0, trongSoDvhd: 50, trongSoGvhd: 50 };

function formatDate(value) {
  return value ? new Intl.DateTimeFormat('vi-VN').format(new Date(value)) : '-';
}

function toDateInput(value) {
  if (!value) return '';
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? '' : date.toISOString().slice(0, 10);
}

function addMonths(value, months) {
  const date = new Date(value);
  date.setMonth(date.getMonth() + months);
  return date.toISOString().slice(0, 10);
}

function mapPeriod(item) {
  return {
    id: item.idKiThucTap,
    name: item.tenKiThucTap,
    startDate: formatDate(item.timeBatDau),
    endDate: formatDate(item.timeKetThuc),
    rawStartDate: item.timeBatDau,
    rawEndDate: item.timeKetThuc,
    status: item.trangThai || '-',
    idTieuChi: item.idTieuChi ?? null,
  };
}

function createDefaultPeriodForm() {
  const today = new Date().toISOString().slice(0, 10);
  return {
    tenKiThucTap: '',
    timeBatDau: today,
    timeKetThuc: addMonths(today, 2),
    trangThai: 'Đang mở',
    idTieuChi: '',
  };
}

function createDefaultCriteriaForm(idKhoa = 0) {
  return {
    tenTieuChi: '',
    idKhoa,
    phanTramChang1: 30,
    phanTramChang2: 70,
    clos: [
      { ...emptyClo, tenClo: 'CLO1', trongSoHp: 50 },
      { ...emptyClo, tenClo: 'CLO2', trongSoHp: 50 },
    ],
  };
}

function createDefaultDvhdForm() {
  return {
    tenDonViHD: '',
    gmailDonViHD: '',
  };
}

function mapCriteriaToForm(criteria, idKhoa) {
  return {
    tenTieuChi: criteria.tenTieuChi || '',
    idKhoa,
    phanTramChang1: Number(criteria.phanTramChang1 ?? 30),
    phanTramChang2: Number(criteria.phanTramChang2 ?? 70),
    clos: (criteria.clos || []).map((clo) => ({
      tenClo: clo.tenClo || '',
      moTaClo: clo.moTaClo || '',
      trongSoHp: Number(clo.trongSoHocPhan ?? 0),
      trongSoDvhd: Number(clo.trongSoDonViHuongDan ?? 0),
      trongSoGvhd: Number(clo.trongSoGiangVienHuongDan ?? 0),
    })),
  };
}

function formatScore(value) {
  return value === null || value === undefined ? '-' : Number(value).toFixed(1);
}

function getStudentMssv(student) {
  return student?.mssv ?? student?.mSSV ?? student?.MSSV ?? '';
}

function getMinhChungUrl(path) {
  if (!path) return '#';
  return path.startsWith('http') ? path : `${API_BASE_URL}${path}`;
}

function KhoaDashboard({ account, onLogout }) {
  const [khoa, setKhoa] = useState(null);
  const [periods, setPeriods] = useState([]);
  const [criteriaList, setCriteriaList] = useState([]);
  const [students, setStudents] = useState([]);
  const [allStudents, setAllStudents] = useState([]);
  const [teachers, setTeachers] = useState([]);
  const [teacherCounts, setTeacherCounts] = useState([]);
  const [dvhdList, setDvhdList] = useState([]);
  const [selectedPeriod, setSelectedPeriod] = useState(null);
  const [selectedStudent, setSelectedStudent] = useState(null);
  const [scoreResult, setScoreResult] = useState(null);
  const [scoreEditOpen, setScoreEditOpen] = useState(false);
  const [scoreEditMeta, setScoreEditMeta] = useState({
    chang: '1',
    nguoiChamLoai: 'GVHD',
  });
  const [scoreEditValues, setScoreEditValues] = useState({});
  const [scoreSaving, setScoreSaving] = useState(false);
  const [minhChungList, setMinhChungList] = useState([]);
  const [minhChungLoading, setMinhChungLoading] = useState(false);
  const [minhChungModalOpen, setMinhChungModalOpen] = useState(false);
  const [periodSearch, setPeriodSearch] = useState('');
  const [studentSearch, setStudentSearch] = useState('');
  const [studentListSearch, setStudentListSearch] = useState('');
  const [activeMenu, setActiveMenu] = useState('periods');
  const [studentMenuOpen, setStudentMenuOpen] = useState(false);
  const [notice, setNotice] = useState(null);
  const [loading, setLoading] = useState(true);
  const [studentLoading, setStudentLoading] = useState(false);
  const [scoreLoading, setScoreLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [studentListPage, setStudentListPage] = useState(1);
  const [periodForm, setPeriodForm] = useState(createDefaultPeriodForm);
  const [criteriaForm, setCriteriaForm] = useState(createDefaultCriteriaForm);
  const [editingPeriod, setEditingPeriod] = useState(null);
  const [editingCriteria, setEditingCriteria] = useState(null);
  const [editingDvhd, setEditingDvhd] = useState(null);
  const [creatingPeriod, setCreatingPeriod] = useState(false);
  const [creatingCriteria, setCreatingCriteria] = useState(false);
  const [creatingDvhd, setCreatingDvhd] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deletingId, setDeletingId] = useState(null);
  const [dvhdLoading, setDvhdLoading] = useState(false);
  const [dvhdForm, setDvhdForm] = useState(createDefaultDvhdForm);
  const [editingInternStudent, setEditingInternStudent] = useState(null);
  const [assigningTeacher, setAssigningTeacher] = useState(false);
  const [gvhdForm, setGvhdForm] = useState('');
  const [importingStudents, setImportingStudents] = useState(false);
  const [previewingImport, setPreviewingImport] = useState(false);
  const [importPreview, setImportPreview] = useState(null);
  const [importFile, setImportFile] = useState(null);
  const importInputRef = useRef(null);

  const pageSize = 9;
  const totalPages = Math.max(1, Math.ceil(periods.length / pageSize));
  const visiblePeriods = useMemo(() => periods.slice((page - 1) * pageSize, page * pageSize), [page, periods]);
  const studentListPageSize = 8;
  const periodModalMode = creatingPeriod ? 'create' : editingPeriod ? 'edit' : null;
  const criteriaModalMode = creatingCriteria ? 'create' : editingCriteria ? 'edit' : null;
  const dvhdModalMode = creatingDvhd ? 'create' : editingDvhd ? 'edit' : null;
  const activeCriteria = criteriaList.find((criteria) => criteria.idTieuChi === selectedPeriod?.idTieuChi);

  const filteredPeriodOptions = useMemo(() => {
    const keyword = periodSearch.trim().toLowerCase();
    if (!keyword) return periods;
    return periods.filter((period) => period.name.toLowerCase().includes(keyword));
  }, [periodSearch, periods]);

  const filteredStudents = useMemo(() => {
    const keyword = studentSearch.trim().toLowerCase();
    if (!keyword) return students;
    return students.filter((student) =>
      `${student.mssv} ${student.mssv} ${student.tenSinhVien}`.toLowerCase().includes(keyword)
    );
  }, [studentSearch, students]);

  const khoaTeachers = useMemo(
    () => teachers.filter((teacher) => Number(teacher.idKhoa) === Number(khoa?.idKhoa)),
    [teachers, khoa]
  );

  const internRows = useMemo(
    () => filteredStudents.map((student) => {
      const mssv = student.mssv ?? student.mSSV ?? student.MSSV ?? student.mSSv ?? '';
      const studentInfo = allStudents.find((item) =>
        String(item.mssv ?? item.mSSV ?? item.MSSV ?? '').toLowerCase() === String(mssv).toLowerCase()
      );

      return {
        ...student,
        mssv,
        tenSinhVien: student.tenSinhVien || studentInfo?.tenSinhVien || '',
        gmailSinhVien: student.gmailSinhVien || studentInfo?.gmailSinhVien || '',
        khoa: khoa?.tenKhoa || studentInfo?.idKhoa || '',
        tenDonViHD: student.tenDonViHD || '',
        maSoGiangVien: student.maSoGiangVien || '',
        tenGiangVien: student.tenGiangVien || '',
      };
    }),
    [allStudents, filteredStudents, khoa]
  );

  const teacherRows = useMemo(
    () => khoaTeachers.map((teacher) => {
      const countItem = teacherCounts.find((item) =>
        String(item.maSoGiangVien || item.MaSoGiangVien || '').toLowerCase() === String(teacher.maSoGiangVien || '').toLowerCase()
      );

      return {
        ...teacher,
        assignedCount: Number(countItem?.soSinhVien ?? countItem?.SoSinhVien ?? 0),
      };
    }),
    [khoaTeachers, teacherCounts]
  );

  const filteredStudentDirectory = useMemo(() => {
    const keyword = studentListSearch.trim().toLowerCase();
    const byKhoa = allStudents.filter((student) => Number(student.idKhoa) === Number(khoa?.idKhoa));

    if (!keyword) {
      return byKhoa;
    }

    return byKhoa.filter((student) =>
      `${student.mssv} ${student.tenSinhVien} ${student.gmailSinhVien}`.toLowerCase().includes(keyword)
    );
  }, [allStudents, khoa, studentListSearch]);

  const studentListTotalPages = Math.max(1, Math.ceil(filteredStudentDirectory.length / studentListPageSize));
  const visibleStudentDirectory = useMemo(
    () => filteredStudentDirectory.slice((studentListPage - 1) * studentListPageSize, studentListPage * studentListPageSize),
    [filteredStudentDirectory, studentListPage]
  );

  useEffect(() => {
    let mounted = true;
    async function loadData() {
      setLoading(true);
      setNotice(null);
      try {
        const khoaResult = await getCurrentKhoa(account?.email);
        if (!mounted) return;
        setKhoa(khoaResult);
        if (!khoaResult?.idKhoa) {
          setNotice({ type: 'error', text: 'Không tìm thấy thông tin khoa.' });
          return;
        }
        const [periodResult, criteriaResult, studentResult, teacherResult, teacherCountResult] = await Promise.all([
          getKiThucTapByKhoa(khoaResult.idKhoa),
          getTieuChiByKhoa(khoaResult.idKhoa),
          getAllSinhVien(),
          getAllGiangVien(),
          getGiangVienPhuTrachCounts(),
        ]);
        if (!mounted) return;
        const mappedPeriods = Array.isArray(periodResult) ? periodResult.map(mapPeriod) : [];
        setPeriods(mappedPeriods);
        setCriteriaList(Array.isArray(criteriaResult) ? criteriaResult : []);
        setAllStudents(Array.isArray(studentResult) ? studentResult : []);
        setTeachers(Array.isArray(teacherResult) ? teacherResult : []);
        setTeacherCounts(Array.isArray(teacherCountResult) ? teacherCountResult : []);
        setSelectedPeriod(mappedPeriods[0] || null);
      } catch (error) {
        if (mounted) setNotice({ type: 'error', text: error.message });
      } finally {
        if (mounted) setLoading(false);
      }
    }
    loadData();
    return () => {
      mounted = false;
    };
  }, []);

  useEffect(() => {
    let mounted = true;
    async function loadStudents() {
      if (!selectedPeriod?.id) {
        setStudents([]);
        setSelectedStudent(null);
        return;
      }
      setStudentLoading(true);
      setScoreResult(null);
      try {
        const result = await getSinhVienThucTapByKi(selectedPeriod.id);
        if (!mounted) return;
        const list = Array.isArray(result) ? result : [];
        setStudents(list);
        setSelectedStudent(list[0] || null);
      } catch (error) {
        if (mounted) {
          setStudents([]);
          setSelectedStudent(null);
          setNotice({ type: 'error', text: error.message });
        }
      } finally {
        if (mounted) setStudentLoading(false);
      }
    }
    loadStudents();
    return () => {
      mounted = false;
    };
  }, [selectedPeriod]);

  useEffect(() => {
    let mounted = true;
    async function loadDvhd() {
      if (!selectedPeriod?.id) {
        setDvhdList([]);
        return;
      }

      setDvhdLoading(true);
      try {
        const result = await getDonViHuongDanByKi(selectedPeriod.id);
        if (mounted) setDvhdList(Array.isArray(result) ? result : []);
      } catch (error) {
        if (mounted) {
          setDvhdList([]);
          setNotice({ type: 'error', text: error.message });
        }
      } finally {
        if (mounted) setDvhdLoading(false);
      }
    }

    loadDvhd();

    return () => {
      mounted = false;
    };
  }, [selectedPeriod]);

  useEffect(() => {
    setStudentListPage(1);
  }, [studentListSearch]);

  useEffect(() => {
    let mounted = true;
    async function loadScore() {
      const mssv = getStudentMssv(selectedStudent);
      if (!selectedPeriod?.id || !mssv) {
        setScoreResult(null);
        return;
      }
      setScoreLoading(true);
      try {
        const result = await getKetQuaDanhGia(selectedPeriod.id, mssv);
        if (mounted) setScoreResult(result);
      } catch {
        if (mounted) setScoreResult(null);
      } finally {
        if (mounted) setScoreLoading(false);
      }
    }
    loadScore();
    return () => {
      mounted = false;
    };
  }, [selectedPeriod, selectedStudent]);

  useEffect(() => {
    let mounted = true;

    async function loadMinhChung() {
      const mssv = getStudentMssv(selectedStudent);

      if (!mssv) {
        setMinhChungList([]);
        return;
      }

      setMinhChungLoading(true);

      try {
        const result = await getMinhChungByMssv(mssv);
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

  async function handleLogout() {
    try {
      await logout();
    } finally {
      onLogout?.();
    }
  }

  function getScoreRowsForEdit() {
    if (scoreResult?.ketQuaClo?.length) {
      return scoreResult.ketQuaClo;
    }

    return (activeCriteria?.clos || []).map((clo) => ({
      maClo: clo.tenClo,
      tenClo: clo.tenClo,
      moTaClo: clo.moTaClo,
    }));
  }

  function getExistingScoreForEdit(clo, meta) {
    const chang = Number(meta.chang);
    const nguoiChamLoai = meta.nguoiChamLoai;

    if (chang === 1 && nguoiChamLoai === 'DVHD') {
      return clo.diemChang1Dvhd;
    }

    if (chang === 1 && nguoiChamLoai === 'GVHD') {
      return clo.diemChang1Gvhd;
    }

    if (chang === 2 && nguoiChamLoai === 'DVHD') {
      return clo.diemChang2Dvhd;
    }

    return clo.diemChang2Gvhd;
  }

  function buildScoreEditValues(meta) {
    return getScoreRowsForEdit().reduce((values, clo) => {
      const maClo = clo.maClo || clo.tenClo;
      const score = getExistingScoreForEdit(clo, meta);

      values[maClo] = score === null || score === undefined ? '' : String(score);

      return values;
    }, {});
  }

  function openScoreEditModal() {
    const mssv = getStudentMssv(selectedStudent);

    if (!selectedPeriod?.id || !mssv) {
      setNotice({ type: 'error', text: 'Vui long chon ky thuc tap va sinh vien can sua diem.' });
      return;
    }

    const nextMeta = {
      chang: '1',
      nguoiChamLoai: 'GVHD',
    };

    setScoreEditMeta(nextMeta);
    setScoreEditValues(buildScoreEditValues(nextMeta));
    setScoreEditOpen(true);
    setNotice(null);
  }

  function updateScoreEditMeta(field, value) {
    const nextMeta = {
      ...scoreEditMeta,
      [field]: value,
    };

    setScoreEditMeta(nextMeta);
    setScoreEditValues(buildScoreEditValues(nextMeta));
  }

  function updateScoreEditValue(maClo, value) {
    setScoreEditValues((current) => ({
      ...current,
      [maClo]: value,
    }));
  }

  async function handleSubmitScoreEdit(event) {
    event.preventDefault();

    const mssv = getStudentMssv(selectedStudent);
    const rows = getScoreRowsForEdit();
    const diemClo = rows.map((clo) => {
      const maClo = clo.maClo || clo.tenClo;

      return {
        maClo,
        diem: Number(scoreEditValues[maClo]),
      };
    });

    if (!selectedPeriod?.id || !mssv || diemClo.length === 0) {
      setNotice({ type: 'error', text: 'Thieu thong tin ky thuc tap, sinh vien hoac tieu chi danh gia.' });
      return;
    }

    if (diemClo.some((item) => Number.isNaN(item.diem) || item.diem < 0 || item.diem > 10)) {
      setNotice({ type: 'error', text: 'Diem CLO phai nam trong khoang 0 den 10.' });
      return;
    }

    setScoreSaving(true);
    setNotice(null);

    try {
      await capNhatDiemThucTapTuKhoa({
        mssv,
        idKiThucTap: selectedPeriod.id,
        chang: Number(scoreEditMeta.chang),
        nguoiChamLoai: scoreEditMeta.nguoiChamLoai,
        nguoiChamId: `KHOA-${khoa?.idKhoa || account?.idKhoa || '0'}`,
        diemClo,
      });

      const result = await getKetQuaDanhGia(selectedPeriod.id, mssv);
      setScoreResult(result);
      setScoreEditOpen(false);
      setNotice({ type: 'success', text: 'Da cap nhat diem sinh vien.' });
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setScoreSaving(false);
    }
  }

  function selectPeriod(period) {
    setSelectedPeriod(period);
    setPeriodSearch(period.name);
  }

  function openCreatePeriod() {
    setCreatingPeriod(true);
    setEditingPeriod(null);
    setPeriodForm(createDefaultPeriodForm());
    setNotice(null);
  }

  function openEditPeriod(period) {
    setEditingPeriod(period);
    setCreatingPeriod(false);
    setPeriodForm({
      tenKiThucTap: period.name === '-' ? '' : period.name,
      timeBatDau: toDateInput(period.rawStartDate),
      timeKetThuc: toDateInput(period.rawEndDate),
      trangThai: period.status === '-' ? 'Đang mở' : period.status,
      idTieuChi: period.idTieuChi ?? '',
    });
    setNotice(null);
  }

  function openCreateCriteria() {
    setCreatingCriteria(true);
    setEditingCriteria(null);
    setCriteriaForm(createDefaultCriteriaForm(khoa?.idKhoa || 0));
    setNotice(null);
  }

  function openCreateDvhd() {
    if (!selectedPeriod?.id) {
      setNotice({ type: 'error', text: 'Vui lòng chọn kỳ thực tập trước khi thêm ĐVHD.' });
      return;
    }

    setCreatingDvhd(true);
    setEditingDvhd(null);
    setDvhdForm(createDefaultDvhdForm());
    setNotice(null);
  }

  function openEditDvhd(dvhd) {
    setEditingDvhd(dvhd);
    setCreatingDvhd(false);
    setDvhdForm({
      tenDonViHD: dvhd.tenDonViHD || '',
      gmailDonViHD: dvhd.gmailDonViHD || '',
    });
    setNotice(null);
  }

  function openEditCriteria(criteria) {
    setEditingCriteria(criteria);
    setCreatingCriteria(false);
    setCriteriaForm(mapCriteriaToForm(criteria, khoa?.idKhoa || criteria.idKhoa));
    setNotice(null);
  }

  function closePeriodModal() {
    if (!saving) {
      setCreatingPeriod(false);
      setEditingPeriod(null);
    }
  }

  function closeCriteriaModal() {
    if (!saving) {
      setCreatingCriteria(false);
      setEditingCriteria(null);
    }
  }

  function closeDvhdModal() {
    if (!saving) {
      setCreatingDvhd(false);
      setEditingDvhd(null);
    }
  }

  function openAssignTeacher(student) {
    setEditingInternStudent(student);
    setGvhdForm(student.maSoGiangVien || '');
    setNotice(null);
  }

  function closeAssignTeacherModal() {
    if (!assigningTeacher) {
      setEditingInternStudent(null);
      setGvhdForm('');
    }
  }

  async function handleAssignTeacherSubmit(event) {
    event.preventDefault();

    if (!selectedPeriod?.id || !editingInternStudent?.mssv || !gvhdForm) {
      setNotice({ type: 'error', text: 'Vui lòng chọn đầy đủ kỳ thực tập, sinh viên và GVHD.' });
      return;
    }

    setAssigningTeacher(true);
    try {
      const response = await phanCongGVHD({
        idKiThucTap: selectedPeriod.id,
        mssv: editingInternStudent.mssv,
        maSoGiangVien: gvhdForm,
      });

      const selectedTeacher = teachers.find((teacher) => teacher.maSoGiangVien === gvhdForm);
      const previousTeacherCode = editingInternStudent.maSoGiangVien || '';
      setStudents((current) => current.map((student) => {
        const mssv = student.mssv ?? student.mSSV ?? student.MSSV ?? '';
        if (String(mssv).toLowerCase() !== String(editingInternStudent.mssv).toLowerCase()) {
          return student;
        }

        return {
          ...student,
          maSoGiangVien: gvhdForm,
          tenGiangVien: selectedTeacher?.tenGiangVien || student.tenGiangVien,
        };
      }));
      setTeacherCounts((current) => {
        const applyDelta = (items, teacherCode, delta) => {
          if (!teacherCode) return items;
          const found = items.some((item) =>
            String(item.maSoGiangVien || item.MaSoGiangVien || '').toLowerCase() === String(teacherCode).toLowerCase()
          );

          if (!found && delta > 0) {
            return [...items, { maSoGiangVien: teacherCode, soSinhVien: delta }];
          }

          return items.map((item) => {
            const itemCode = item.maSoGiangVien || item.MaSoGiangVien || '';
            if (String(itemCode).toLowerCase() !== String(teacherCode).toLowerCase()) {
              return item;
            }

            const currentCount = Number(item.soSinhVien ?? item.SoSinhVien ?? 0);
            return { ...item, soSinhVien: Math.max(0, currentCount + delta) };
          });
        };

        let next = current;
        if (previousTeacherCode && previousTeacherCode !== gvhdForm) {
          next = applyDelta(next, previousTeacherCode, -1);
        }
        if (!previousTeacherCode || previousTeacherCode !== gvhdForm) {
          next = applyDelta(next, gvhdForm, 1);
        }
        return next;
      });
      setNotice({ type: 'success', text: response?.message || 'Phân công GVHD thành công.' });
      setEditingInternStudent(null);
      setGvhdForm('');
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setAssigningTeacher(false);
    }
  }

  function openImportFilePicker() {
    if (!selectedPeriod?.id) {
      setNotice({ type: 'error', text: 'Vui lòng chọn kỳ thực tập trước khi import sinh viên.' });
      return;
    }

    importInputRef.current?.click();
  }

  async function handleImportStudents(event) {
    const file = event.target.files?.[0];
    event.target.value = '';

    if (!file) return;

    if (!selectedPeriod?.id) {
      setNotice({ type: 'error', text: 'Vui lòng chọn kỳ thực tập trước khi import sinh viên.' });
      return;
    }

    setPreviewingImport(true);
    setNotice(null);
    try {
      const result = await previewImportSinhVienThucTap(selectedPeriod.id, file);
      setImportFile(file);
      setImportPreview(result);
    } catch (error) {
      setImportFile(null);
      setImportPreview(null);
      setNotice({ type: 'error', text: error.message });
    } finally {
      setPreviewingImport(false);
    }
  }

  function closeImportPreviewModal() {
    if (!importingStudents) {
      setImportFile(null);
      setImportPreview(null);
    }
  }

  async function saveImportStudents() {
    if (!selectedPeriod?.id || !importFile) {
      setNotice({ type: 'error', text: 'Không tìm thấy file import hoặc kỳ thực tập.' });
      return;
    }

    setImportingStudents(true);
    setNotice(null);
    try {
      const result = await importSinhVienThucTap(selectedPeriod.id, importFile);
      const [updatedStudents, updatedTeacherCounts] = await Promise.all([
        getSinhVienThucTapByKi(selectedPeriod.id),
        getGiangVienPhuTrachCounts(),
      ]);
      setStudents(Array.isArray(updatedStudents) ? updatedStudents : []);
      setTeacherCounts(Array.isArray(updatedTeacherCounts) ? updatedTeacherCounts : []);
      setSelectedStudent(Array.isArray(updatedStudents) ? updatedStudents[0] || null : null);

      const successCount = result?.soDongThanhCong ?? result?.SoDongThanhCong;
      const totalCount = result?.tongSoDong ?? result?.TongSoDong;
      const detail = successCount !== undefined && totalCount !== undefined
        ? ` (${successCount}/${totalCount} dòng thành công)`
        : '';

      setNotice({ type: 'success', text: `Import sinh viên thực tập thành công${detail}.` });
      setImportFile(null);
      setImportPreview(null);
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setImportingStudents(false);
    }
  }

  function handlePeriodFormChange(event) {
    const { name, value } = event.target;
    setPeriodForm((current) => ({ ...current, [name]: value }));
  }

  function handleCriteriaChange(event) {
    const { name, value } = event.target;
    setCriteriaForm((current) => ({ ...current, [name]: name.startsWith('phanTram') ? Number(value) : value }));
  }

  function handleDvhdFormChange(event) {
    const { name, value } = event.target;
    setDvhdForm((current) => ({ ...current, [name]: value }));
  }

  function handleCloChange(index, event) {
    const { name, value } = event.target;
    setCriteriaForm((current) => ({
      ...current,
      clos: current.clos.map((clo, cloIndex) =>
        cloIndex === index ? { ...clo, [name]: name.startsWith('trongSo') ? Number(value) : value } : clo
      ),
    }));
  }

  function addCloRow() {
    setCriteriaForm((current) => ({
      ...current,
      clos: [...current.clos, { ...emptyClo, tenClo: `CLO${current.clos.length + 1}` }],
    }));
  }

  function removeCloRow(index) {
    setCriteriaForm((current) => ({ ...current, clos: current.clos.filter((_, cloIndex) => cloIndex !== index) }));
  }

  function validatePeriodForm() {
    if (!periodForm.tenKiThucTap.trim()) return 'Tên kỳ thực tập không được để trống.';
    if (new Date(periodForm.timeKetThuc) < new Date(periodForm.timeBatDau)) {
      return 'Ngày kết thúc không được trước ngày bắt đầu.';
    }
    return null;
  }

  function validateCriteriaForm() {
    if (!criteriaForm.tenTieuChi.trim()) return 'Tên tiêu chí không được để trống.';
    if (criteriaForm.clos.length === 0) return 'Cần có ít nhất một CLO.';
    if (Math.abs(criteriaForm.phanTramChang1 + criteriaForm.phanTramChang2 - 100) > 0.01) {
      return 'Tổng phần trăm chặng 1 và chặng 2 phải bằng 100.';
    }
    const totalHp = criteriaForm.clos.reduce((sum, clo) => sum + Number(clo.trongSoHp || 0), 0);
    if (Math.abs(totalHp - 100) > 0.01) return 'Tổng trọng số học phần của các CLO phải bằng 100.';
    for (const clo of criteriaForm.clos) {
      if (!clo.tenClo.trim()) return 'Tên CLO không được để trống.';
      if (Math.abs(Number(clo.trongSoDvhd) + Number(clo.trongSoGvhd) - 100) > 0.01) {
        return `Tổng trọng số ĐVHD và GVHD của ${clo.tenClo} phải bằng 100.`;
      }
    }
    return null;
  }

  async function handlePeriodSubmit(event) {
    event.preventDefault();
    const validationMessage = validatePeriodForm();
    if (validationMessage) return setNotice({ type: 'error', text: validationMessage });
    setSaving(true);
    setNotice(null);
    try {
      const payload = {
        ...periodForm,
        idTieuChi: periodForm.idTieuChi ? Number(periodForm.idTieuChi) : null,
        idKhoa: khoa.idKhoa,
      };
      if (creatingPeriod) {
        const result = await createKiThucTap(payload);
        const newPeriod = mapPeriod({
          idKiThucTap: result?.id_ki_thuc_tap ?? result?.idKiThucTap ?? Date.now(),
          tenKiThucTap: payload.tenKiThucTap,
          timeBatDau: payload.timeBatDau,
          timeKetThuc: payload.timeKetThuc,
          trangThai: payload.trangThai,
          idTieuChi: payload.idTieuChi,
        });
        setPeriods((current) => [newPeriod, ...current]);
        setSelectedPeriod(newPeriod);
        setNotice({ type: 'success', text: 'Tạo kỳ thực tập thành công.' });
      } else {
        await updateKiThucTap(editingPeriod.id, payload);
        const updatedPeriod = mapPeriod({
          idKiThucTap: editingPeriod.id,
          tenKiThucTap: payload.tenKiThucTap,
          timeBatDau: payload.timeBatDau,
          timeKetThuc: payload.timeKetThuc,
          trangThai: payload.trangThai,
          idTieuChi: payload.idTieuChi,
        });
        setPeriods((current) => current.map((period) => (period.id === editingPeriod.id ? updatedPeriod : period)));
        if (selectedPeriod?.id === editingPeriod.id) setSelectedPeriod(updatedPeriod);
        setNotice({ type: 'success', text: 'Cập nhật kỳ thực tập thành công.' });
      }
      closePeriodModal();
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setSaving(false);
    }
  }

  async function handleCriteriaSubmit(event) {
    event.preventDefault();
    const validationMessage = validateCriteriaForm();
    if (validationMessage) return setNotice({ type: 'error', text: validationMessage });
    setSaving(true);
    setNotice(null);
    const payload = { ...criteriaForm, idKhoa: khoa.idKhoa };
    const toCriteria = (idTieuChi) => ({
      idTieuChi,
      tenTieuChi: payload.tenTieuChi,
      idKhoa: payload.idKhoa,
      phanTramChang1: payload.phanTramChang1,
      phanTramChang2: payload.phanTramChang2,
      clos: payload.clos.map((clo, index) => ({
        idClo: Date.now() + index,
        tenClo: clo.tenClo,
        moTaClo: clo.moTaClo,
        trongSoHocPhan: clo.trongSoHp,
        trongSoDonViHuongDan: clo.trongSoDvhd,
        trongSoGiangVienHuongDan: clo.trongSoGvhd,
      })),
    });
    try {
      if (creatingCriteria) {
        const result = await createTieuChi(payload);
        setCriteriaList((current) => [toCriteria(result?.id_tieu_chi ?? result?.idTieuChi ?? Date.now()), ...current]);
        setNotice({ type: 'success', text: 'Tạo tiêu chí đánh giá thành công.' });
      } else {
        await updateTieuChi(editingCriteria.idTieuChi, payload);
        setCriteriaList((current) =>
          current.map((item) => (item.idTieuChi === editingCriteria.idTieuChi ? toCriteria(editingCriteria.idTieuChi) : item))
        );
        setNotice({ type: 'success', text: 'Cập nhật tiêu chí đánh giá thành công.' });
      }
      closeCriteriaModal();
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setSaving(false);
    }
  }

  async function handleDvhdSubmit(event) {
    event.preventDefault();

    if (!selectedPeriod?.id) {
      setNotice({ type: 'error', text: 'Vui lòng chọn kỳ thực tập trước khi lưu ĐVHD.' });
      return;
    }

    if (!dvhdForm.tenDonViHD.trim()) {
      setNotice({ type: 'error', text: 'Tên ĐVHD không được để trống.' });
      return;
    }

    setSaving(true);
    setNotice(null);
    try {
      if (creatingDvhd) {
        const response = await createDonViHuongDan({
          idKiThucTap: selectedPeriod.id,
          tenDonViHD: dvhdForm.tenDonViHD.trim(),
          gmailDonViHD: dvhdForm.gmailDonViHD.trim(),
        });

        setDvhdList((current) => [
          ...current,
          {
            idDonViHD: response?.id_don_vi_hd ?? response?.idDonViHD,
            idKiThucTap: selectedPeriod.id,
            tenDonViHD: dvhdForm.tenDonViHD.trim(),
            gmailDonViHD: dvhdForm.gmailDonViHD.trim(),
          },
        ].sort((left, right) => left.tenDonViHD.localeCompare(right.tenDonViHD)));
        setNotice({ type: 'success', text: response?.message || 'Thêm ĐVHD thành công.' });
      } else if (editingDvhd) {
        const response = await updateDonViHuongDan(editingDvhd.idDonViHD, {
          tenDonViHD: dvhdForm.tenDonViHD.trim(),
          gmailDonViHD: dvhdForm.gmailDonViHD.trim(),
        });

        setDvhdList((current) => current.map((item) =>
          item.idDonViHD === editingDvhd.idDonViHD
            ? { ...item, tenDonViHD: dvhdForm.tenDonViHD.trim(), gmailDonViHD: dvhdForm.gmailDonViHD.trim() }
            : item
        ));
        setNotice({ type: 'success', text: response?.message || 'Cập nhật ĐVHD thành công.' });
      }

      closeDvhdModal();
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setSaving(false);
    }
  }

  async function handleDeletePeriod(period) {
    if (!window.confirm(`Bạn có chắc muốn xóa "${period.name}" không?`)) return;
    setDeletingId(`period-${period.id}`);
    try {
      await deleteKiThucTap(period.id);
      setPeriods((current) => current.filter((item) => item.id !== period.id));
      if (selectedPeriod?.id === period.id) setSelectedPeriod(periods.find((item) => item.id !== period.id) || null);
      setNotice({ type: 'success', text: 'Xóa kỳ thực tập thành công.' });
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setDeletingId(null);
    }
  }

  async function handleDeleteCriteria(criteria) {
    if (!window.confirm(`Bạn có chắc muốn xóa "${criteria.tenTieuChi}" không?`)) return;
    setDeletingId(`criteria-${criteria.idTieuChi}`);
    try {
      await deleteTieuChi(criteria.idTieuChi);
      setCriteriaList((current) => current.filter((item) => item.idTieuChi !== criteria.idTieuChi));
      setNotice({ type: 'success', text: 'Xóa tiêu chí đánh giá thành công.' });
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setDeletingId(null);
    }
  }

  async function handleDeleteDvhd(dvhd) {
    if (!window.confirm(`Bạn có chắc muốn xóa ĐVHD "${dvhd.tenDonViHD}" không?`)) return;

    setDeletingId(`dvhd-${dvhd.idDonViHD}`);
    setNotice(null);
    try {
      const response = await deleteDonViHuongDan(dvhd.idDonViHD);
      setDvhdList((current) => current.filter((item) => item.idDonViHD !== dvhd.idDonViHD));
      setNotice({ type: 'success', text: response?.message || 'Xóa ĐVHD thành công.' });
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setDeletingId(null);
    }
  }

  async function handleDeleteInternStudent(student) {
    if (!selectedPeriod?.id || !student?.mssv) {
      setNotice({ type: 'error', text: 'Không tìm thấy kỳ thực tập hoặc sinh viên cần xóa.' });
      return;
    }

    if (!window.confirm(`Bạn có chắc muốn xóa sinh viên "${student.tenSinhVien || student.mssv}" khỏi kỳ thực tập này không?`)) return;

    setDeletingId(`intern-${selectedPeriod.id}-${student.mssv}`);
    setNotice(null);
    try {
      const response = await deleteSinhVienThucTap(selectedPeriod.id, student.mssv);
      setStudents((current) => current.filter((item) => {
        const currentMssv = item.mssv ?? item.mSSV ?? item.MSSV ?? '';
        return String(currentMssv).toLowerCase() !== String(student.mssv).toLowerCase();
      }));

      const selectedMssv = selectedStudent?.mssv ?? selectedStudent?.mSSV ?? selectedStudent?.MSSV ?? '';
      if (String(selectedMssv).toLowerCase() === String(student.mssv).toLowerCase()) {
        setSelectedStudent(null);
      }

      if (student.maSoGiangVien) {
        setTeacherCounts((current) => current.map((item) => {
          const itemCode = item.maSoGiangVien || item.MaSoGiangVien || '';
          if (String(itemCode).toLowerCase() !== String(student.maSoGiangVien).toLowerCase()) {
            return item;
          }

          const currentCount = Number(item.soSinhVien ?? item.SoSinhVien ?? 0);
          return { ...item, soSinhVien: Math.max(0, currentCount - 1) };
        }));
      }

      setNotice({ type: 'success', text: response?.message || 'Xóa sinh viên thực tập thành công.' });
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setDeletingId(null);
    }
  }

  function renderStudentDirectory() {
    return (
      <section className="student-directory">
        <div className="student-directory-search">
          <input
            value={studentListSearch}
            onChange={(event) => setStudentListSearch(event.target.value)}
            placeholder="Nhập tên sinh viên hoặc mã số sinh viên"
          />
        </div>

        <div className="student-directory-table-wrap">
          <table className="student-directory-table">
            <thead>
              <tr>
                <th>MSSV</th>
                <th>Tên sinh viên</th>
                <th>Khoa</th>
                <th>Mail</th>
              </tr>
            </thead>
            <tbody>
              {visibleStudentDirectory.map((student) => (
                <tr key={student.mssv}>
                  <td>{student.mssv}</td>
                  <td>{student.tenSinhVien}</td>
                  <td>{khoa?.tenKhoa || student.idKhoa}</td>
                  <td>{student.gmailSinhVien}</td>
                </tr>
              ))}

              {visibleStudentDirectory.length === 0 && (
                <tr>
                  <td colSpan="4">Không tìm thấy sinh viên trong khoa.</td>
                </tr>
              )}
            </tbody>
          </table>

          <footer className="student-directory-pagination">
            <button
              type="button"
              disabled={studentListPage === 1}
              onClick={() => setStudentListPage((current) => current - 1)}
            >
              &lt;
            </button>
            <span>{studentListPage}</span>
            <button
              type="button"
              disabled={studentListPage === studentListTotalPages}
              onClick={() => setStudentListPage((current) => current + 1)}
            >
              &gt;
            </button>
          </footer>
        </div>
      </section>
    );
  }

  function renderInternStudents() {
    return (
      <section className="intern-students">
        <div className="intern-search-row">
          <div className="search-box">
            <label>Tìm kiếm kỳ thực tập</label>
            <input
              value={periodSearch}
              onChange={(event) => setPeriodSearch(event.target.value)}
              placeholder="Nhập kỳ thực tập"
            />
            {periodSearch && (
              <div className="period-search-results">
                {filteredPeriodOptions.map((period) => (
                  <button key={period.id} type="button" onClick={() => selectPeriod(period)}>
                    <strong>{period.name}</strong>
                    <span>{period.startDate} - {period.endDate}</span>
                  </button>
                ))}
                {filteredPeriodOptions.length === 0 && <span>Không tìm thấy kỳ thực tập.</span>}
              </div>
            )}
          </div>
          <div className="selected-period-pill">
            {selectedPeriod ? selectedPeriod.name : 'Chưa có kỳ thực tập'}
          </div>
        </div>

        <div className="student-directory-table-wrap intern-student-table-wrap">
          <table className="student-directory-table intern-student-table">
            <thead>
              <tr>
                <th>MSSV</th>
                <th>Họ và tên</th>
                <th>Khoa</th>
                <th>Mail</th>
                <th>ĐVHD</th>
                <th>GVHD</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {studentLoading && (
                <tr>
                  <td colSpan="7">Đang tải sinh viên thực tập...</td>
                </tr>
              )}

              {!studentLoading && internRows.map((student) => (
                <tr key={`${selectedPeriod?.id}-${student.mssv}`}>
                  <td>{student.mssv}</td>
                  <td>{student.tenSinhVien || '-'}</td>
                  <td>{student.khoa || '-'}</td>
                  <td>{student.gmailSinhVien || '-'}</td>
                  <td>{student.tenDonViHD || '-'}</td>
                  <td>{student.tenGiangVien || 'Chưa phân công'}</td>
                  <td>
                    <div className="table-action-group">
                      <button className="table-action" type="button" onClick={() => openAssignTeacher(student)}>
                        Sửa
                      </button>
                      <button
                        className="table-action danger"
                        type="button"
                        disabled={deletingId === `intern-${selectedPeriod?.id}-${student.mssv}`}
                        onClick={() => handleDeleteInternStudent(student)}
                      >
                        {deletingId === `intern-${selectedPeriod?.id}-${student.mssv}` ? 'Đang xóa...' : 'Xóa'}
                      </button>
                    </div>
                  </td>
                </tr>
              ))}

              {!studentLoading && internRows.length === 0 && (
                <tr>
                  <td colSpan="7">Chưa có sinh viên thực tập trong kỳ này.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        <div className="intern-student-actions">
          <input
            ref={importInputRef}
            className="file-input-hidden"
            type="file"
            accept=".xlsx,.xls"
            onChange={handleImportStudents}
          />
          <button type="button" onClick={openImportFilePicker} disabled={previewingImport || importingStudents || !selectedPeriod?.id}>
            {previewingImport ? 'Đang đọc file...' : 'Import sinh viên'}
          </button>
          <button type="button">Excel</button>
        </div>
      </section>
    );
  }

  function renderTeachers() {
    return (
      <section className="teacher-management">
        <div className="teacher-summary-grid">
          <div>
            <span>Tổng giảng viên</span>
            <strong>{teacherRows.length}</strong>
          </div>
          <div>
            <span>Đã phụ trách</span>
            <strong>{teacherRows.filter((teacher) => teacher.assignedCount > 0).length}</strong>
          </div>
          <div>
            <span>Sinh viên đã phân công</span>
            <strong>{teacherRows.reduce((total, teacher) => total + teacher.assignedCount, 0)}</strong>
          </div>
        </div>

        <div className="student-directory-table-wrap teacher-table-wrap">
          <table className="student-directory-table teacher-table">
            <thead>
              <tr>
                <th>Mã GV</th>
                <th>Tên giảng viên</th>
                <th>Khoa</th>
                <th>Email</th>
                <th>Số SV đang phụ trách</th>
                <th>Trạng thái tải</th>
              </tr>
            </thead>
            <tbody>
              {loading && (
                <tr>
                  <td colSpan="6">Đang tải danh sách giảng viên...</td>
                </tr>
              )}

              {!loading && teacherRows.map((teacher) => (
                <tr key={teacher.maSoGiangVien}>
                  <td>{teacher.maSoGiangVien}</td>
                  <td>{teacher.tenGiangVien}</td>
                  <td>{khoa?.tenKhoa || teacher.idKhoa}</td>
                  <td>{teacher.gmailGiangVien || '-'}</td>
                  <td>
                    <span className={teacher.assignedCount >= 5 ? 'teacher-load full' : 'teacher-load'}>
                      {teacher.assignedCount}/5
                    </span>
                  </td>
                  <td>{teacher.assignedCount >= 5 ? 'Đã đủ 5 sinh viên' : 'Còn nhận sinh viên'}</td>
                </tr>
              ))}

              {!loading && teacherRows.length === 0 && (
                <tr>
                  <td colSpan="6">Chưa có giảng viên trong khoa.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>
    );
  }

  function renderDvhdManagement() {
    return (
      <section className="dvhd-management">
        <div className="intern-search-row">
          <div className="search-box">
            <label>Tìm kiếm kỳ thực tập</label>
            <input
              value={periodSearch}
              onChange={(event) => setPeriodSearch(event.target.value)}
              placeholder="Nhập tên kỳ thực tập"
            />
            {periodSearch && (
              <div className="period-search-results">
                {filteredPeriodOptions.map((period) => (
                  <button key={period.id} type="button" onClick={() => selectPeriod(period)}>
                    <strong>{period.name}</strong>
                    <span>{period.startDate} - {period.endDate}</span>
                  </button>
                ))}
                {filteredPeriodOptions.length === 0 && <span>Không tìm thấy kỳ thực tập.</span>}
              </div>
            )}
          </div>
          <div className="selected-period-pill">
            {selectedPeriod ? selectedPeriod.name : 'Chưa có kỳ thực tập'}
          </div>
        </div>

        <div className="dvhd-actions">
          <button type="button" onClick={openCreateDvhd} disabled={!selectedPeriod?.id}>+ Thêm ĐVHD</button>
        </div>

        <div className="student-directory-table-wrap dvhd-table-wrap">
          <table className="student-directory-table dvhd-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Tên ĐVHD</th>
                <th>Email</th>
                <th>Kỳ thực tập</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {dvhdLoading && (
                <tr>
                  <td colSpan="5">Đang tải danh sách ĐVHD...</td>
                </tr>
              )}

              {!dvhdLoading && dvhdList.map((dvhd) => (
                <tr key={dvhd.idDonViHD}>
                  <td>{dvhd.idDonViHD}</td>
                  <td>{dvhd.tenDonViHD}</td>
                  <td>{dvhd.gmailDonViHD || '-'}</td>
                  <td>{selectedPeriod?.name || dvhd.idKiThucTap}</td>
                  <td>
                    <div className="table-action-group">
                      <button className="table-action" type="button" onClick={() => openEditDvhd(dvhd)}>Sửa</button>
                      <button
                        className="table-action danger"
                        type="button"
                        disabled={deletingId === `dvhd-${dvhd.idDonViHD}`}
                        onClick={() => handleDeleteDvhd(dvhd)}
                      >
                        {deletingId === `dvhd-${dvhd.idDonViHD}` ? 'Đang xóa...' : 'Xóa'}
                      </button>
                    </div>
                  </td>
                </tr>
              ))}

              {!dvhdLoading && dvhdList.length === 0 && (
                <tr>
                  <td colSpan="5">Chưa có ĐVHD trong kỳ thực tập này.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>
    );
  }

  function renderInternManagement() {
    return (
      <section className="intern-workspace">
        <div className="intern-search-row">
          <div className="search-box">
            <label>Tìm kiếm kỳ thực tập</label>
            <input
              value={periodSearch}
              onChange={(event) => setPeriodSearch(event.target.value)}
              placeholder="Nhập tên kỳ thực tập"
            />
            {periodSearch && (
              <div className="period-search-results">
                {filteredPeriodOptions.map((period) => (
                  <button key={period.id} type="button" onClick={() => selectPeriod(period)}>
                    <strong>{period.name}</strong>
                    <span>{period.startDate} - {period.endDate}</span>
                  </button>
                ))}
                {filteredPeriodOptions.length === 0 && <span>Không tìm thấy kỳ thực tập.</span>}
              </div>
            )}
          </div>
          <div className="selected-period-pill">
            {selectedPeriod ? selectedPeriod.name : 'Chưa có kỳ thực tập'}
          </div>
        </div>

        <div className="intern-layout">
          <aside className="student-panel">
            <input
              value={studentSearch}
              onChange={(event) => setStudentSearch(event.target.value)}
              placeholder="Nhập MSSV hoặc tên"
            />
            <div className="student-list">
              {studentLoading && <div className="student-empty">Đang tải sinh viên...</div>}
              {!studentLoading && filteredStudents.map((student) => {
                const mssv = student.mssv ?? student.mSSV ?? student.MSSV;
                return (
                  <button
                    key={mssv}
                    type="button"
                    className={selectedStudent && (selectedStudent.mssv ?? selectedStudent.mSSV ?? selectedStudent.MSSV) === mssv ? 'active' : ''}
                    onClick={() => setSelectedStudent(student)}
                  >
                    <strong>{student.tenSinhVien}</strong>
                    <span>MSSV: {mssv}</span>
                  </button>
                );
              })}
              {!studentLoading && filteredStudents.length === 0 && <div className="student-empty">Chưa có sinh viên.</div>}
            </div>
          </aside>

          <section className="score-panel">
            <div className="score-summary">
              <div>
                <span>Sinh viên</span>
                <strong>{selectedStudent?.tenSinhVien || 'Chưa chọn sinh viên'}</strong>
              </div>
              <div>
                <span>Tiêu chí</span>
                <strong>{activeCriteria?.tenTieuChi || 'Kỳ này chưa chọn tiêu chí'}</strong>
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

            <div className="score-inline-actions">
              <button type="button" onClick={openScoreEditModal}>Sua diem</button>
            </div>

            <div className="score-table-wrap">
              <table className="score-table">
                <thead>
                  <tr>
                    <th rowSpan="2">CLO</th>
                    <th rowSpan="2">Mô tả năng lực đầu ra</th>
                    <th colSpan="3">Điểm Chặng 1</th>
                    <th colSpan="3">Điểm Chặng 2</th>
                    <th rowSpan="2">Điểm chuẩn đầu ra</th>
                    <th rowSpan="2">Trọng số</th>
                    <th rowSpan="2">Điểm quy đổi</th>
                  </tr>
                  <tr>
                    <th>ĐVHD</th>
                    <th>GVHD</th>
                    <th>TB</th>
                    <th>ĐVHD</th>
                    <th>GVHD</th>
                    <th>TB</th>
                  </tr>
                </thead>
                <tbody>
                  {scoreLoading && (
                    <tr>
                      <td colSpan="11">Đang tải bảng điểm...</td>
                    </tr>
                  )}
                  {!scoreLoading && (scoreResult?.ketQuaClo || []).map((clo) => (
                    <tr key={clo.maClo}>
                      <td>{clo.maClo}</td>
                      <td>{clo.moTaClo || clo.tenClo}</td>
                      <td>{formatScore(clo.diemChang1Dvhd)}</td>
                      <td>{formatScore(clo.diemChang1Gvhd)}</td>
                      <td>{formatScore(clo.diemChang1)}</td>
                      <td>{formatScore(clo.diemChang2Dvhd)}</td>
                      <td>{formatScore(clo.diemChang2Gvhd)}</td>
                      <td>{formatScore(clo.diemChang2)}</td>
                      <td>{formatScore(clo.diemClo)}</td>
                      <td>{Math.round(Number(clo.trongSoHocPhan || 0) * 100)}%</td>
                      <td>{formatScore(clo.diemQuyDoi)}</td>
                    </tr>
                  ))}
                  {!scoreLoading && (!scoreResult?.ketQuaClo || scoreResult.ketQuaClo.length === 0) && (
                    <tr>
                      <td colSpan="11">Chưa có dữ liệu điểm hoặc kỳ chưa chọn tiêu chí đánh giá.</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>

            <div
              className="score-actions"
              onClick={(event) => {
                if (
                  event.target.tagName === 'BUTTON'
                  && event.target.textContent?.toLowerCase().includes('minh')
                ) {
                  setMinhChungModalOpen(true);
                }
              }}
            >
              <button type="button">Xem minh chứng</button>
              <button type="button" onClick={() => window.print()}>Xuất excel</button>
            </div>
          </section>
        </div>
      </section>
    );
  }

  function renderPeriods() {
    return (
      <>
        <section className="period-grid" aria-label="Danh sách kỳ thực tập">
          {visiblePeriods.map((period) => (
            <article className="period-card" key={period.id}>
              <div className="period-card-top"><span className="period-status">{period.status}</span></div>
              <h2>{period.name}</h2>
              <dl>
                <div><dt>Tiêu chí</dt><dd>{criteriaList.find((criteria) => criteria.idTieuChi === period.idTieuChi)?.tenTieuChi || 'Chưa chọn'}</dd></div>
                <div><dt>Bắt đầu</dt><dd>{period.startDate}</dd></div>
                <div><dt>Kết thúc</dt><dd>{period.endDate}</dd></div>
              </dl>
              <div className="card-actions">
                <button className="edit-action" type="button" onClick={() => openEditPeriod(period)}>Sửa</button>
                <button className="delete-action" type="button" disabled={deletingId === `period-${period.id}`} onClick={() => handleDeletePeriod(period)}>
                  {deletingId === `period-${period.id}` ? 'Đang xóa...' : 'Xóa'}
                </button>
              </div>
            </article>
          ))}
          {!loading && visiblePeriods.length === 0 && <div className="period-empty">Chưa có kỳ thực tập trong database.</div>}
        </section>
        <footer className="khoa-pagination">
          <button type="button" disabled={page === 1} onClick={() => setPage((current) => current - 1)}>Trước</button>
          <span>Trang {page}/{totalPages}</span>
          <button type="button" disabled={page === totalPages} onClick={() => setPage((current) => current + 1)}>Sau</button>
        </footer>
      </>
    );
  }

  function renderCriteria() {
    return (
      <section className="criteria-list" aria-label="Danh sách tiêu chí đánh giá">
        {criteriaList.map((criteria) => (
          <article className="criteria-card" key={criteria.idTieuChi}>
            <div className="criteria-card-header">
              <div>
                <span className="period-status">Chặng 1 {criteria.phanTramChang1}% - Chặng 2 {criteria.phanTramChang2}%</span>
                <h2>{criteria.tenTieuChi}</h2>
              </div>
              <div className="card-actions compact">
                <button className="edit-action" type="button" onClick={() => openEditCriteria(criteria)}>Sửa</button>
                <button className="delete-action" type="button" disabled={deletingId === `criteria-${criteria.idTieuChi}`} onClick={() => handleDeleteCriteria(criteria)}>
                  {deletingId === `criteria-${criteria.idTieuChi}` ? 'Đang xóa...' : 'Xóa'}
                </button>
              </div>
            </div>
            <div className="criteria-table">
              <div className="criteria-table-head"><span>CLO</span><span>Trọng số HP</span><span>ĐVHD</span><span>GVHD</span></div>
              {(criteria.clos || []).map((clo) => (
                <div className="criteria-table-row" key={clo.idClo || clo.tenClo}>
                  <span>{clo.tenClo}</span><span>{clo.trongSoHocPhan}%</span><span>{clo.trongSoDonViHuongDan}%</span><span>{clo.trongSoGiangVienHuongDan}%</span>
                </div>
              ))}
            </div>
          </article>
        ))}
        {!loading && criteriaList.length === 0 && <div className="period-empty">Chưa có tiêu chí đánh giá trong database.</div>}
      </section>
    );
  }

  const toolbarTitle = activeMenu === 'criteria'
    ? 'Quản lý tiêu chí chấm điểm'
    : activeMenu === 'studentList'
      ? 'Danh sách sinh viên'
    : activeMenu === 'interns'
      ? 'Sinh viên thực tập'
    : activeMenu === 'scores'
      ? 'Quản lý điểm thực tập'
    : activeMenu === 'teachers'
      ? 'Quản lý giảng viên'
    : activeMenu === 'dvhd'
      ? 'Quản lý ĐVHD'
      : 'Quản lý kỳ thực tập của khoa';
  const importPreviewRows = importPreview?.ketQua ?? importPreview?.KetQua ?? [];
  const importPreviewSuccessCount = importPreview?.soDongThanhCong ?? importPreview?.SoDongThanhCong ?? 0;
  const importPreviewErrorCount = importPreview?.soDongLoi ?? importPreview?.SoDongLoi ?? 0;

  return (
    <main className="khoa-dashboard">
      <aside className="khoa-sidebar">
        <div className="khoa-identity">
          <span>Khoa</span>
          <strong>{loading ? 'Đang tải...' : khoa?.tenKhoa || 'Chưa có dữ liệu'}</strong>
          {account?.email && <small>{account.email}</small>}
        </div>
        <nav className="khoa-menu" aria-label="Điều hướng khoa">
          <button type="button" className={activeMenu === 'periods' ? 'active' : ''} onClick={() => setActiveMenu('periods')}>Quản lý kỳ thực tập</button>
          <button
            type="button"
            className={activeMenu === 'studentList' || activeMenu === 'interns' ? 'active' : ''}
            onClick={() => setStudentMenuOpen((current) => !current)}
          >
            Quản lý sinh viên
          </button>
          {studentMenuOpen && (
            <div className="khoa-submenu">
              <button type="button" className={activeMenu === 'studentList' ? 'active' : ''} onClick={() => setActiveMenu('studentList')}>
                - Danh sách sinh viên
              </button>
              <button type="button" className={activeMenu === 'interns' ? 'active' : ''} onClick={() => setActiveMenu('interns')}>
                - Sinh viên thực tập
              </button>
            </div>
          )}
          <button type="button" className={activeMenu === 'scores' ? 'active' : ''} onClick={() => setActiveMenu('scores')}>Quản lý điểm thực tập</button>
          <button type="button" className={activeMenu === 'criteria' ? 'active' : ''} onClick={() => setActiveMenu('criteria')}>Quản lý tiêu chí</button>
          <button type="button" className={activeMenu === 'teachers' ? 'active' : ''} onClick={() => setActiveMenu('teachers')}>Quản lý giảng viên</button>
          <button type="button" className={activeMenu === 'dvhd' ? 'active' : ''} onClick={() => setActiveMenu('dvhd')}>Quản lý ĐVHD</button>
        </nav>
      </aside>

      <section className="khoa-main">
        <header className="khoa-toolbar">
          <div>
            <span className="toolbar-kicker">{activeMenu === 'scores' || activeMenu === 'interns' || activeMenu === 'dvhd' ? selectedPeriod?.name || 'Kỳ thực tập mới nhất' : 'Khoa'}</span>
            <h1>{toolbarTitle}</h1>
          </div>
          <div className="toolbar-actions">
            {activeMenu !== 'interns' && activeMenu !== 'studentList' && activeMenu !== 'scores' && activeMenu !== 'teachers' && activeMenu !== 'dvhd' && (
              <button className="add-period-button" type="button" onClick={activeMenu === 'criteria' ? openCreateCriteria : openCreatePeriod}>
                {activeMenu === 'criteria' ? '+ Thêm tiêu chí' : '+ Thêm kỳ thực tập'}
              </button>
            )}
            <button className="khoa-profile-toggle" type="button" onClick={handleLogout}>Đăng xuất</button>
          </div>
        </header>

        {notice && <div className={`khoa-notice ${notice.type}`}>{notice.text}</div>}
        {activeMenu === 'studentList' && renderStudentDirectory()}
        {activeMenu === 'interns' && renderInternStudents()}
        {activeMenu === 'scores' && renderInternManagement()}
        {activeMenu === 'criteria' && renderCriteria()}
        {activeMenu === 'teachers' && renderTeachers()}
        {activeMenu === 'dvhd' && renderDvhdManagement()}
        {activeMenu === 'periods' && renderPeriods()}
      </section>

      {minhChungModalOpen && (
        <div className="edit-modal-backdrop" role="presentation" onMouseDown={() => setMinhChungModalOpen(false)}>
          <section className="edit-modal evidence-modal" onMouseDown={(event) => event.stopPropagation()}>
            <div className="edit-modal-header">
              <span>Minh chứng sinh viên</span>
              <button type="button" aria-label="Đóng" onClick={() => setMinhChungModalOpen(false)}>x</button>
            </div>

            <div className="evidence-modal-summary">
              <div>
                <span>Sinh viên</span>
                <strong>{selectedStudent?.tenSinhVien || '-'}</strong>
              </div>
              <div>
                <span>MSSV</span>
                <strong>{getStudentMssv(selectedStudent) || '-'}</strong>
              </div>
              <div>
                <span>Số minh chứng</span>
                <strong>{minhChungList.length}</strong>
              </div>
            </div>

            <div className="evidence-list">
              {minhChungLoading && <div className="student-empty">Đang tải minh chứng...</div>}

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
                <div className="student-empty">Sinh viên này chưa nộp minh chứng.</div>
              )}
            </div>
          </section>
        </div>
      )}

      {importPreview && (
        <div className="edit-modal-backdrop" role="presentation" onMouseDown={closeImportPreviewModal}>
          <section className="edit-modal import-preview-modal" onMouseDown={(event) => event.stopPropagation()}>
            <div className="edit-modal-header">
              <span>Xem trước import sinh viên</span>
              <button type="button" aria-label="Đóng" onClick={closeImportPreviewModal} disabled={importingStudents}>x</button>
            </div>

            <div className="import-preview-summary">
              <div>
                <span>File</span>
                <strong>{importFile?.name || '-'}</strong>
              </div>
              <div>
                <span>Hợp lệ</span>
                <strong>{importPreviewSuccessCount}</strong>
              </div>
              <div>
                <span>Không hợp lệ</span>
                <strong>{importPreviewErrorCount}</strong>
              </div>
            </div>

            <div className="import-preview-table-wrap">
              <table className="import-preview-table">
                <thead>
                  <tr>
                    <th>Dòng</th>
                    <th>MSSV</th>
                    <th>Tên sinh viên</th>
                    <th>ĐVHD</th>
                    <th>Trạng thái</th>
                    <th>Thông báo</th>
                  </tr>
                </thead>
                <tbody>
                  {importPreviewRows.map((row) => {
                    const isValid = row.thanhCong ?? row.ThanhCong;
                    return (
                      <tr key={`${row.dong ?? row.Dong}-${row.mssv ?? row.MSSV}`} className={isValid ? 'valid' : 'invalid'}>
                        <td>{row.dong ?? row.Dong}</td>
                        <td>{row.mssv ?? row.MSSV}</td>
                        <td>{row.hoTen ?? row.HoTen}</td>
                        <td>{row.tenDonViHD ?? row.TenDonViHD}</td>
                        <td>{isValid ? 'Có trong DB' : 'Không hợp lệ'}</td>
                        <td>{row.thongBao ?? row.ThongBao}</td>
                      </tr>
                    );
                  })}
                  {importPreviewRows.length === 0 && (
                    <tr>
                      <td colSpan="6">File không có dữ liệu để import.</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>

            <div className="edit-modal-actions">
              <button type="button" className="cancel-action" onClick={closeImportPreviewModal} disabled={importingStudents}>Hủy</button>
              <button type="button" className="save-action" onClick={saveImportStudents} disabled={importingStudents || importPreviewSuccessCount === 0}>
                {importingStudents ? 'Đang lưu...' : 'Lưu xuống CSDL'}
              </button>
            </div>
          </section>
        </div>
      )}

      {dvhdModalMode && (
        <div className="edit-modal-backdrop" role="presentation" onMouseDown={closeDvhdModal}>
          <form className="edit-modal" onSubmit={handleDvhdSubmit} onMouseDown={(event) => event.stopPropagation()}>
            <div className="edit-modal-header">
              <span>{dvhdModalMode === 'create' ? 'Thêm ĐVHD' : 'Chỉnh sửa ĐVHD'}</span>
              <button type="button" aria-label="Đóng" onClick={closeDvhdModal} disabled={saving}>x</button>
            </div>
            <label>Tên ĐVHD
              <input name="tenDonViHD" value={dvhdForm.tenDonViHD} onChange={handleDvhdFormChange} required />
            </label>
            <label>Email
              <input name="gmailDonViHD" type="email" value={dvhdForm.gmailDonViHD} onChange={handleDvhdFormChange} />
            </label>
            <div className="edit-modal-actions">
              <button type="button" className="cancel-action" onClick={closeDvhdModal} disabled={saving}>Hủy</button>
              <button type="submit" className="save-action" disabled={saving}>{saving ? 'Đang lưu...' : 'Lưu'}</button>
            </div>
          </form>
        </div>
      )}

      {periodModalMode && (
        <div className="edit-modal-backdrop" role="presentation" onMouseDown={closePeriodModal}>
          <form className="edit-modal" onSubmit={handlePeriodSubmit} onMouseDown={(event) => event.stopPropagation()}>
            <div className="edit-modal-header">
              <span>{periodModalMode === 'create' ? 'Thêm kỳ thực tập' : 'Chỉnh sửa kỳ thực tập'}</span>
              <button type="button" aria-label="Đóng" onClick={closePeriodModal} disabled={saving}>x</button>
            </div>
            <label>Tên kỳ thực tập<input name="tenKiThucTap" value={periodForm.tenKiThucTap} onChange={handlePeriodFormChange} required /></label>
            <div className="edit-date-row">
              <label>Ngày bắt đầu<input name="timeBatDau" type="date" value={periodForm.timeBatDau} onChange={handlePeriodFormChange} required /></label>
              <label>Ngày kết thúc<input name="timeKetThuc" type="date" value={periodForm.timeKetThuc} onChange={handlePeriodFormChange} required /></label>
            </div>
            <label>Trạng thái
              <select name="trangThai" value={periodForm.trangThai} onChange={handlePeriodFormChange} required>
                <option value="">Chọn trạng thái</option>
                <option value="Đang mở">Đang mở</option>
                <option value="Đang diễn ra">Đang diễn ra</option>
                <option value="Đã kết thúc">Đã kết thúc</option>
                <option value="Tạm đóng">Tạm đóng</option>
              </select>
            </label>
            <label>Tiêu chí đánh giá
              <select name="idTieuChi" value={periodForm.idTieuChi} onChange={handlePeriodFormChange}>
                <option value="">Chưa chọn tiêu chí</option>
                {criteriaList.map((criteria) => <option key={criteria.idTieuChi} value={criteria.idTieuChi}>{criteria.tenTieuChi}</option>)}
              </select>
            </label>
            <div className="edit-modal-actions">
              <button type="button" className="cancel-action" onClick={closePeriodModal} disabled={saving}>Hủy</button>
              <button type="submit" className="save-action" disabled={saving}>{saving ? 'Đang lưu...' : 'Lưu'}</button>
            </div>
          </form>
        </div>
      )}

      {criteriaModalMode && (
        <div className="edit-modal-backdrop" role="presentation" onMouseDown={closeCriteriaModal}>
          <form className="edit-modal criteria-modal" onSubmit={handleCriteriaSubmit} onMouseDown={(event) => event.stopPropagation()}>
            <div className="edit-modal-header">
              <span>{criteriaModalMode === 'create' ? 'Thêm tiêu chí đánh giá' : 'Chỉnh sửa tiêu chí đánh giá'}</span>
              <button type="button" aria-label="Đóng" onClick={closeCriteriaModal} disabled={saving}>x</button>
            </div>
            <label>Tên tiêu chí<input name="tenTieuChi" value={criteriaForm.tenTieuChi} onChange={handleCriteriaChange} required /></label>
            <div className="edit-date-row">
              <label>% Chặng 1<input name="phanTramChang1" type="number" min="0" max="100" value={criteriaForm.phanTramChang1} onChange={handleCriteriaChange} required /></label>
              <label>% Chặng 2<input name="phanTramChang2" type="number" min="0" max="100" value={criteriaForm.phanTramChang2} onChange={handleCriteriaChange} required /></label>
            </div>
            <div className="clo-editor-header"><span>Danh sách CLO</span><button type="button" className="inline-action" onClick={addCloRow}>+ Thêm CLO</button></div>
            <div className="clo-editor-list">
              {criteriaForm.clos.map((clo, index) => (
                <div className="clo-editor-row" key={`${clo.tenClo}-${index}`}>
                  <label>Tên CLO<input name="tenClo" value={clo.tenClo} onChange={(event) => handleCloChange(index, event)} required /></label>
                  <label>Mô tả<input name="moTaClo" value={clo.moTaClo} onChange={(event) => handleCloChange(index, event)} /></label>
                  <label>% HP<input name="trongSoHp" type="number" min="0" max="100" value={clo.trongSoHp} onChange={(event) => handleCloChange(index, event)} required /></label>
                  <label>% ĐVHD<input name="trongSoDvhd" type="number" min="0" max="100" value={clo.trongSoDvhd} onChange={(event) => handleCloChange(index, event)} required /></label>
                  <label>% GVHD<input name="trongSoGvhd" type="number" min="0" max="100" value={clo.trongSoGvhd} onChange={(event) => handleCloChange(index, event)} required /></label>
                  <button type="button" className="remove-clo-action" onClick={() => removeCloRow(index)} disabled={criteriaForm.clos.length === 1}>Xóa</button>
                </div>
              ))}
            </div>
            <div className="edit-modal-actions">
              <button type="button" className="cancel-action" onClick={closeCriteriaModal} disabled={saving}>Hủy</button>
              <button type="submit" className="save-action" disabled={saving}>{saving ? 'Đang lưu...' : 'Lưu'}</button>
            </div>
          </form>
        </div>
      )}

      {scoreEditOpen && (
        <div className="edit-modal-backdrop" role="presentation">
          <form className="edit-modal score-edit-modal" onSubmit={handleSubmitScoreEdit}>
            <div className="edit-modal-header">
              <span>Sua diem thuc tap</span>
              <button type="button" aria-label="Dong" onClick={() => setScoreEditOpen(false)} disabled={scoreSaving}>x</button>
            </div>

            <div className="score-edit-summary">
              <div>
                <span>Sinh vien</span>
                <strong>{selectedStudent?.tenSinhVien || '-'}</strong>
              </div>
              <div>
                <span>MSSV</span>
                <strong>{getStudentMssv(selectedStudent) || '-'}</strong>
              </div>
              <div>
                <span>Ky thuc tap</span>
                <strong>{selectedPeriod?.name || '-'}</strong>
              </div>
            </div>

            <div className="score-edit-controls">
              <label>Chang danh gia
                <select
                  value={scoreEditMeta.chang}
                  onChange={(event) => updateScoreEditMeta('chang', event.target.value)}
                  disabled={scoreSaving}
                >
                  <option value="1">Chang 1</option>
                  <option value="2">Chang 2</option>
                </select>
              </label>

              <label>Nguoi cham
                <select
                  value={scoreEditMeta.nguoiChamLoai}
                  onChange={(event) => updateScoreEditMeta('nguoiChamLoai', event.target.value)}
                  disabled={scoreSaving}
                >
                  <option value="GVHD">Giang vien huong dan</option>
                  <option value="DVHD">Don vi huong dan</option>
                </select>
              </label>
            </div>

            <div className="score-edit-list">
              {getScoreRowsForEdit().map((clo) => {
                const maClo = clo.maClo || clo.tenClo;

                return (
                  <label className="score-edit-row" key={maClo}>
                    <span>
                      <strong>{maClo}</strong>
                      <small>{clo.moTaClo || clo.tenClo}</small>
                    </span>
                    <input
                      type="number"
                      min="0"
                      max="10"
                      step="0.1"
                      value={scoreEditValues[maClo] ?? ''}
                      onChange={(event) => updateScoreEditValue(maClo, event.target.value)}
                      disabled={scoreSaving}
                      required
                    />
                  </label>
                );
              })}
            </div>

            <div className="edit-modal-actions">
              <button type="button" className="cancel-action" onClick={() => setScoreEditOpen(false)} disabled={scoreSaving}>Huy</button>
              <button type="submit" className="save-action" disabled={scoreSaving}>{scoreSaving ? 'Dang luu...' : 'Luu diem'}</button>
            </div>
          </form>
        </div>
      )}

      {editingInternStudent && (
        <div className="edit-modal-backdrop" role="presentation" onMouseDown={closeAssignTeacherModal}>
          <form className="edit-modal intern-assign-modal" onSubmit={handleAssignTeacherSubmit} onMouseDown={(event) => event.stopPropagation()}>
            <div className="edit-modal-header">
              <span>Phân công GVHD</span>
              <button type="button" aria-label="Đóng" onClick={closeAssignTeacherModal} disabled={assigningTeacher}>x</button>
            </div>
            <label>MSSV<input value={editingInternStudent.mssv || ''} readOnly /></label>
            <label>Họ và tên<input value={editingInternStudent.tenSinhVien || ''} readOnly /></label>
            <div className="edit-date-row">
              <label>Khoa<input value={editingInternStudent.khoa || ''} readOnly /></label>
              <label>Mail<input value={editingInternStudent.gmailSinhVien || ''} readOnly /></label>
            </div>
            <label>Đơn vị hướng dẫn<input value={editingInternStudent.tenDonViHD || ''} readOnly /></label>
            <label>GVHD
              <select value={gvhdForm} onChange={(event) => setGvhdForm(event.target.value)} required>
                <option value="">Chọn GVHD</option>
                {khoaTeachers.map((teacher) => (
                  <option key={teacher.maSoGiangVien} value={teacher.maSoGiangVien}>
                    {teacher.tenGiangVien} - {teacher.maSoGiangVien}
                  </option>
                ))}
              </select>
            </label>
            <div className="edit-modal-actions">
              <button type="button" className="cancel-action" onClick={closeAssignTeacherModal} disabled={assigningTeacher}>Hủy</button>
              <button type="submit" className="save-action" disabled={assigningTeacher}>{assigningTeacher ? 'Đang lưu...' : 'Lưu GVHD'}</button>
            </div>
          </form>
        </div>
      )}
    </main>
  );
}

export default KhoaDashboard;
