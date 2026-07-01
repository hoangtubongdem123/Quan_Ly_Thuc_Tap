import { apiRequest } from './api';

export function loginKhoa({ email, password }) {
  return apiRequest('/api/Auth/login-khoa', {
    method: 'POST',
    body: JSON.stringify({
      gmailKhoa: email,
      passwordKhoa: password,
    }),
  });
}

export function loginSinhVien({ email, password }) {
  return apiRequest('/api/Auth/login-sinhvien', {
    method: 'POST',
    body: JSON.stringify({
      gmailSinhVien: email,
      passwordSinhVien: password,
    }),
  });
}

export function loginGiangVien({ email, password }) {
  return apiRequest('/api/Auth/login-giangvien', {
    method: 'POST',
    body: JSON.stringify({
      gmailGiangVien: email,
      passwordGiangVien: password,
    }),
  });
}

export function loginDonViHuongDan({ email, password }) {
  return apiRequest('/api/Auth/login-dvhd', {
    method: 'POST',
    body: JSON.stringify({
      gmailDonViHD: email,
      passwordDonViHD: password,
    }),
  });
}

export function loginAdmin({ username, password }) {
  return apiRequest('/api/Auth/login-admin', {
    method: 'POST',
    body: JSON.stringify({
      taiKhoanAdmin: username,
      passwordAdmin: password,
    }),
  });
}

export function logout() {
  return apiRequest('/api/Auth/logout', {
    method: 'POST',
  });
}
