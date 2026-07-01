import { apiRequest } from './api';

export function getCurrentSinhVien() {
  return apiRequest('/api/SinhVien/Me');
}

export function getMyKiThucTap() {
  return apiRequest('/api/SinhVien/Me/KiThucTap');
}

export function getMyThongBao() {
  return apiRequest('/api/SinhVien/Me/ThongBao');
}

export function getKetQuaDanhGiaSinhVien(idKiThucTap, mssv) {
  return apiRequest(`/api/DanhGiaThucTap/KetQua/${idKiThucTap}/${encodeURIComponent(mssv)}`);
}

export function getMinhChungByMssv(mssv) {
  return apiRequest(`/api/MinhChung/${encodeURIComponent(mssv)}`);
}

export function addMinhChung({ tenMinhChung, mssv, file }) {
  const formData = new FormData();
  formData.append('TenMinhChung', tenMinhChung);
  formData.append('MSSV', mssv);
  formData.append('File', file);

  return apiRequest('/api/MinhChung', {
    method: 'POST',
    body: formData,
  });
}

export function updateMinhChung(idMinhChung, { tenMinhChung, file }) {
  const formData = new FormData();
  formData.append('TenMinhChung', tenMinhChung);

  if (file) {
    formData.append('File', file);
  }

  return apiRequest(`/api/MinhChung/${idMinhChung}`, {
    method: 'PUT',
    body: formData,
  });
}

export function deleteMinhChung(idMinhChung) {
  return apiRequest(`/api/MinhChung/${idMinhChung}`, {
    method: 'DELETE',
  });
}
