import { useEffect, useMemo, useState } from 'react';
import { logout } from '../../services/authService';
import {
  createGiangVien,
  createKhoa,
  createSinhVien,
  deleteGiangVien,
  deleteKhoa,
  deleteSinhVien,
  getAllGiangVien,
  getAllKhoa,
  getAllSinhVien,
  updateGiangVien,
  updateKhoa,
  updateSinhVien,
} from '../../services/adminService';
import './AdminDashboard.css';

const resources = {
  khoa: {
    label: 'Khoa',
    key: 'idKhoa',
    load: getAllKhoa,
    create: createKhoa,
    update: updateKhoa,
    remove: deleteKhoa,
    initialForm: {
      tenKhoa: '',
      gmailKhoa: '',
      passwordKhoa: '',
    },
    fields: [
      { name: 'tenKhoa', label: 'Ten khoa', required: true },
      { name: 'gmailKhoa', label: 'Gmail khoa', type: 'email', required: true },
      { name: 'passwordKhoa', label: 'Mat khau', type: 'password', required: true },
    ],
    columns: [
      { name: 'idKhoa', label: 'ID' },
      { name: 'tenKhoa', label: 'Ten khoa' },
      { name: 'gmailKhoa', label: 'Gmail' },
    ],
  },
  sinhvien: {
    label: 'Sinh vien',
    key: 'mssv',
    load: getAllSinhVien,
    create: createSinhVien,
    update: updateSinhVien,
    remove: deleteSinhVien,
    initialForm: {
      mssv: '',
      idKhoa: '',
      tenSinhVien: '',
      gmailSinhVien: '',
      passwordSinhVien: '',
    },
    fields: [
      { name: 'mssv', label: 'MSSV', required: true },
      { name: 'idKhoa', label: 'Khoa', type: 'khoa', required: true },
      { name: 'tenSinhVien', label: 'Ten sinh vien', required: true },
      { name: 'gmailSinhVien', label: 'Gmail sinh vien', type: 'email', required: true },
      { name: 'passwordSinhVien', label: 'Mat khau', type: 'password', required: true },
    ],
    columns: [
      { name: 'mssv', label: 'MSSV' },
      { name: 'tenSinhVien', label: 'Ten sinh vien' },
      { name: 'idKhoa', label: 'Khoa', type: 'khoa' },
      { name: 'gmailSinhVien', label: 'Gmail' },
    ],
  },
  giangvien: {
    label: 'Giang vien',
    key: 'maSoGiangVien',
    load: getAllGiangVien,
    create: createGiangVien,
    update: updateGiangVien,
    remove: deleteGiangVien,
    initialForm: {
      maSoGiangVien: '',
      idKhoa: '',
      tenGiangVien: '',
      gmailGiangVien: '',
      passwordGiangVien: '',
    },
    fields: [
      { name: 'maSoGiangVien', label: 'Ma so giang vien', required: true },
      { name: 'idKhoa', label: 'Khoa', type: 'khoa', required: true },
      { name: 'tenGiangVien', label: 'Ten giang vien', required: true },
      { name: 'gmailGiangVien', label: 'Gmail giang vien', type: 'email', required: true },
      { name: 'passwordGiangVien', label: 'Mat khau', type: 'password', required: true },
    ],
    columns: [
      { name: 'maSoGiangVien', label: 'Ma GV' },
      { name: 'tenGiangVien', label: 'Ten giang vien' },
      { name: 'idKhoa', label: 'Khoa', type: 'khoa' },
      { name: 'gmailGiangVien', label: 'Gmail' },
    ],
  },
};

function normalizeRows(data, resource) {
  if (!Array.isArray(data)) {
    return [];
  }

  const byKey = new Map();

  data.forEach((item) => {
    const rowKey = item?.[resource.key];

    if (rowKey === null || rowKey === undefined || rowKey === '') {
      return;
    }

    const current = byKey.get(rowKey) || {};
    const merged = { ...current };

    Object.entries(item).forEach(([field, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        merged[field] = value;
      } else if (!(field in merged)) {
        merged[field] = value;
      }
    });

    byKey.set(rowKey, merged);
  });

  return Array.from(byKey.values());
}

function AdminDashboard({ account, onLogout }) {
  const [activeResource, setActiveResource] = useState('khoa');
  const [rows, setRows] = useState([]);
  const [khoaOptions, setKhoaOptions] = useState([]);
  const [form, setForm] = useState(resources.khoa.initialForm);
  const [editingRow, setEditingRow] = useState(null);
  const [keyword, setKeyword] = useState('');
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [notice, setNotice] = useState(null);

  const resource = resources[activeResource];

  const filteredRows = useMemo(() => {
    const normalizedKeyword = keyword.trim().toLowerCase();

    if (!normalizedKeyword) {
      return rows;
    }

    return rows.filter((row) =>
      Object.values(row).some((value) =>
        String(value ?? '').toLowerCase().includes(normalizedKeyword)
      )
    );
  }, [keyword, rows]);

  useEffect(() => {
    loadRows();
  }, [activeResource]);

  useEffect(() => {
    loadKhoaOptions();
  }, []);

  async function loadKhoaOptions() {
    try {
      const data = await getAllKhoa();
      setKhoaOptions(normalizeRows(data, resources.khoa));
    } catch {
      setKhoaOptions([]);
    }
  }

  async function loadRows() {
    setLoading(true);
    setNotice(null);

    try {
      const data = await resource.load();
      const normalizedRows = normalizeRows(data, resource);
      setRows(normalizedRows);

      if (activeResource === 'khoa') {
        setKhoaOptions(normalizedRows);
      }
    } catch (error) {
      setRows([]);
      setNotice({ type: 'error', text: error.message });
    } finally {
      setLoading(false);
    }
  }

  function changeResource(nextResource) {
    setActiveResource(nextResource);
    setEditingRow(null);
    setKeyword('');
    setForm(resources[nextResource].initialForm);
  }

  function updateField(field, value) {
    setForm((current) => ({
      ...current,
      [field]: value,
    }));
  }

  function resetForm() {
    setEditingRow(null);
    setForm(resource.initialForm);
  }

  function startEdit(row) {
    const nextForm = { ...resource.initialForm };

    resource.fields.forEach((field) => {
      nextForm[field.name] = row[field.name] ?? '';
    });

    setEditingRow(row);
    setForm(nextForm);
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setSaving(true);
    setNotice(null);

    const payload = { ...form };

    resource.fields.forEach((field) => {
      if (field.type === 'number' || field.type === 'khoa') {
        payload[field.name] = Number(payload[field.name]);
      } else {
        payload[field.name] = String(payload[field.name] ?? '').trim();
      }
    });

    try {
      if (editingRow) {
        await resource.update(editingRow[resource.key], payload);
        setNotice({ type: 'success', text: `Da cap nhat ${resource.label}.` });
      } else {
        await resource.create(payload);
        setNotice({ type: 'success', text: `Da them ${resource.label}.` });
      }

      resetForm();
      await loadRows();
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(row) {
    if (!confirm(`Xoa ${resource.label} nay?`)) {
      return;
    }

    setNotice(null);

    try {
      await resource.remove(row[resource.key]);
      setNotice({ type: 'success', text: `Da xoa ${resource.label}.` });
      await loadRows();
      await loadKhoaOptions();

      if (editingRow?.[resource.key] === row[resource.key]) {
        resetForm();
      }
    } catch (error) {
      setNotice({ type: 'error', text: error.message });
    }
  }

  function getKhoaName(idKhoa) {
    const khoa = khoaOptions.find((item) => Number(item.idKhoa) === Number(idKhoa));
    return khoa ? `${khoa.tenKhoa} (${khoa.idKhoa})` : idKhoa || '-';
  }

  function getCellValue(row, column) {
    if (column.type === 'khoa') {
      return getKhoaName(row[column.name]);
    }

    return row[column.name] || '-';
  }

  async function handleLogout() {
    try {
      await logout();
    } catch {
      // Local logout is enough when the server session already expired.
    } finally {
      onLogout?.();
    }
  }

  return (
    <main className="admin-dashboard">
      <aside className="admin-dashboard-sidebar">
        <div className="admin-dashboard-identity">
          <span>Admin</span>
          <strong>{account?.username || 'Quan tri vien'}</strong>
          <small>Quan ly du lieu nen he thong</small>
        </div>

        <nav className="admin-dashboard-menu" aria-label="Danh muc quan tri">
          {Object.entries(resources).map(([key, item]) => (
            <button
              key={key}
              className={activeResource === key ? 'active' : ''}
              type="button"
              onClick={() => changeResource(key)}
            >
              {item.label}
            </button>
          ))}
        </nav>

        <button className="admin-dashboard-logout" type="button" onClick={handleLogout}>
          Dang xuat
        </button>
      </aside>

      <section className="admin-dashboard-main">
        <header className="admin-dashboard-toolbar">
          <div>
            <span>Quan tri danh muc</span>
            <h1>{resource.label}</h1>
          </div>
          <input
            className="admin-dashboard-search"
            placeholder="Tim kiem..."
            value={keyword}
            onChange={(event) => setKeyword(event.target.value)}
          />
        </header>

        {notice && (
          <div className={`admin-dashboard-notice ${notice.type}`} role="status">
            {notice.text}
          </div>
        )}

        <div className="admin-dashboard-grid">
          <section className="admin-dashboard-form-panel">
            <h2>{editingRow ? `Sua ${resource.label}` : `Them ${resource.label}`}</h2>

            <form className="admin-dashboard-form" onSubmit={handleSubmit}>
              {resource.fields.map((field) => (
                <label key={field.name}>
                  <span>{field.label}</span>
                  {field.type === 'khoa' ? (
                    <select
                      value={form[field.name] ?? ''}
                      onChange={(event) => updateField(field.name, event.target.value)}
                      required={field.required}
                    >
                      <option value="">Chon khoa</option>
                      {khoaOptions.map((khoa) => (
                        <option key={khoa.idKhoa} value={khoa.idKhoa}>
                          {khoa.tenKhoa} ({khoa.idKhoa})
                        </option>
                      ))}
                    </select>
                  ) : (
                    <input
                      type={field.type || 'text'}
                      value={form[field.name] ?? ''}
                      onChange={(event) => updateField(field.name, event.target.value)}
                      readOnly={Boolean(editingRow) && field.name === resource.key}
                      required={field.required}
                    />
                  )}
                </label>
              ))}

              <div className="admin-dashboard-form-actions">
                <button className="primary" type="submit" disabled={saving}>
                  {saving ? 'Dang luu...' : editingRow ? 'Luu' : 'Them'}
                </button>
                <button className="secondary" type="button" onClick={resetForm}>
                  Lam moi
                </button>
              </div>
            </form>
          </section>

          <section className="admin-dashboard-table-panel">
            <div className="admin-dashboard-table-summary">
              {loading ? 'Dang tai du lieu...' : `${filteredRows.length}/${rows.length} ban ghi`}
            </div>

            <div className="admin-dashboard-table-wrap">
              <table>
                <thead>
                  <tr>
                    {resource.columns.map((column) => (
                      <th key={column.name}>{column.label}</th>
                    ))}
                    <th>Thao tac</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredRows.map((row, index) => (
                    <tr key={`${activeResource}-${row[resource.key]}-${index}`}>
                      {resource.columns.map((column) => (
                        <td key={column.name}>{getCellValue(row, column)}</td>
                      ))}
                      <td>
                        <div className="admin-dashboard-row-actions">
                          <button type="button" className="edit" onClick={() => startEdit(row)}>
                            Sua
                          </button>
                          <button type="button" className="delete" onClick={() => handleDelete(row)}>
                            Xoa
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}

                  {!loading && filteredRows.length === 0 && (
                    <tr>
                      <td colSpan={resource.columns.length + 1} className="admin-dashboard-empty">
                        Khong co du lieu
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </section>
        </div>
      </section>
    </main>
  );
}

export default AdminDashboard;
