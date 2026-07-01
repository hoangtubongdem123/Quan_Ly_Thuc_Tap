import { apiRequest } from './api';

export async function getCurrentKhoa(email) {
  let profile = null;

  if (!email) {
    try {
      profile = await apiRequest('/api/Auth/profile');
    } catch {
      profile = null;
    }
  }

  const khoaList = await apiRequest('/api/Khoa');

  if (!Array.isArray(khoaList) || khoaList.length === 0) {
    return null;
  }

  const gmail = email || profile?.gmail;

  if (gmail) {
    const matchedKhoa = khoaList.find(
      (khoa) => khoa.gmailKhoa?.toLowerCase() === gmail.toLowerCase()
    );

    if (matchedKhoa) {
      return matchedKhoa;
    }
  }

  return khoaList[0];
}

export function getKiThucTapByKhoa(idKhoa) {
  return apiRequest(`/api/Khoa/${idKhoa}/KiThucTap`);
}

export function createKiThucTap(payload) {
  return apiRequest('/api/Khoa/CreateKiThucTap', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export function updateKiThucTap(idKiThucTap, payload) {
  return apiRequest(`/api/Khoa/KiThucTap/${idKiThucTap}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
}

export function deleteKiThucTap(idKiThucTap) {
  return apiRequest(`/api/Khoa/KiThucTap/${idKiThucTap}`, {
    method: 'DELETE',
  });
}

export function getSinhVienThucTapByKi(idKiThucTap) {
  return apiRequest(`/api/Khoa/KiThucTap/${idKiThucTap}/SinhVien`);
}

export function deleteSinhVienThucTap(idKiThucTap, mssv) {
  return apiRequest(`/api/Khoa/KiThucTap/${idKiThucTap}/SinhVien/${encodeURIComponent(mssv)}`, {
    method: 'DELETE',
  });
}

export function getDonViHuongDanByKi(idKiThucTap) {
  return apiRequest(`/api/Khoa/KiThucTap/${idKiThucTap}/DonViHuongDan`);
}

export function createDonViHuongDan(payload) {
  return apiRequest('/api/Khoa/DonViHuongDan', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export function updateDonViHuongDan(idDonViHD, payload) {
  return apiRequest(`/api/Khoa/DonViHuongDan/${idDonViHD}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
}

export function deleteDonViHuongDan(idDonViHD) {
  return apiRequest(`/api/Khoa/DonViHuongDan/${idDonViHD}`, {
    method: 'DELETE',
  });
}

export function importSinhVienThucTap(idKiThucTap, file) {
  const formData = new FormData();
  formData.append('IdKiThucTap', idKiThucTap);
  formData.append('File', file);

  return apiRequest('/api/Khoa/ImportSVThucTap', {
    method: 'POST',
    body: formData,
  });
}

export function previewImportSinhVienThucTap(idKiThucTap, file) {
  const formData = new FormData();
  formData.append('IdKiThucTap', idKiThucTap);
  formData.append('File', file);

  return apiRequest('/api/Khoa/PreviewImportSVThucTap', {
    method: 'POST',
    body: formData,
  });
}

export function getKetQuaDanhGia(idKiThucTap, mssv) {
  return apiRequest(`/api/DanhGiaThucTap/KetQua/${idKiThucTap}/${mssv}`);
}

export function capNhatDiemThucTapTuKhoa(payload) {
  return apiRequest('/api/DanhGiaThucTap/Khoa/CapNhatDiem', {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
}

export function getMinhChungByMssv(mssv) {
  return apiRequest(`/api/MinhChung/${encodeURIComponent(mssv)}`);
}

export function getAllSinhVien() {
  return apiRequest('/api/SinhVien');
}

export function getAllGiangVien() {
  return apiRequest('/api/GiangVien');
}

export function getGiangVienPhuTrachCounts() {
  return apiRequest('/api/GiangVien/SoSinhVienPhuTrach');
}

export function phanCongGVHD(payload) {
  return apiRequest('/api/Khoa/PhanCongGVHD', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export function getTieuChiByKhoa(idKhoa) {
  return apiRequest(`/api/Khoa/${idKhoa}/TieuChi`);
}

export function createTieuChi(payload) {
  return apiRequest('/api/Khoa/TieuChi', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export function updateTieuChi(idTieuChi, payload) {
  return apiRequest(`/api/Khoa/TieuChi/${idTieuChi}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
}

export function deleteTieuChi(idTieuChi) {
  return apiRequest(`/api/Khoa/TieuChi/${idTieuChi}`, {
    method: 'DELETE',
  });
}
