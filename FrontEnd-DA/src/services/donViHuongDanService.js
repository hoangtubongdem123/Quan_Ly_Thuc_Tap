import { apiRequest } from './api';

export function getMySinhVienPhuTrach() {
  return apiRequest('/api/DonViHuongDan/Me/SinhVienPhuTrach');
}

export function getTieuChiDanhGia(idKiThucTap) {
  return apiRequest(`/api/DanhGiaThucTap/TieuChi/${idKiThucTap}`);
}

export function getKetQuaDanhGiaDonVi(idKiThucTap, mssv) {
  return apiRequest(`/api/DanhGiaThucTap/KetQua/${idKiThucTap}/${encodeURIComponent(mssv)}`);
}

export function getMinhChungByMssv(mssv) {
  return apiRequest(`/api/MinhChung/${encodeURIComponent(mssv)}`);
}

export function chamDiemSinhVien(payload) {
  return apiRequest('/api/DanhGiaThucTap/ChamDiem', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}
