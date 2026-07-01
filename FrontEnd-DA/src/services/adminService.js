import { apiRequest } from './api';

export function getAllKhoa() {
  return apiRequest('/api/Khoa');
}

export function createKhoa(payload) {
  return apiRequest('/api/Khoa', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export function updateKhoa(idKhoa, payload) {
  return apiRequest(`/api/Khoa/${idKhoa}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
}

export function deleteKhoa(idKhoa) {
  return apiRequest(`/api/Khoa/${idKhoa}`, {
    method: 'DELETE',
  });
}

export function getAllSinhVien() {
  return apiRequest('/api/SinhVien');
}

export function createSinhVien(payload) {
  return apiRequest('/api/SinhVien', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export function updateSinhVien(mssv, payload) {
  return apiRequest(`/api/SinhVien/${encodeURIComponent(mssv)}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
}

export function deleteSinhVien(mssv) {
  return apiRequest(`/api/SinhVien/${encodeURIComponent(mssv)}`, {
    method: 'DELETE',
  });
}

export function getAllGiangVien() {
  return apiRequest('/api/GiangVien');
}

export function createGiangVien(payload) {
  return apiRequest('/api/GiangVien', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export function updateGiangVien(maSoGiangVien, payload) {
  return apiRequest(`/api/GiangVien/${encodeURIComponent(maSoGiangVien)}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
}

export function deleteGiangVien(maSoGiangVien) {
  return apiRequest(`/api/GiangVien/${encodeURIComponent(maSoGiangVien)}`, {
    method: 'DELETE',
  });
}
