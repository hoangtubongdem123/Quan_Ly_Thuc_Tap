import { useState } from 'react';
import AdminLogin from './pages/Login/AdminLogin';
import DonViHuongDanLogin from './pages/Login/DonViHuongDanLogin';
import GiangVienLogin from './pages/Login/GiangVienLogin';
import KhoaLogin from './pages/Login/KhoaLogin';
import StudentLogin from './pages/Login/StudentLogin';
import KhoaDashboard from './pages/Khoa/KhoaDashboard';
import SinhVienDashboard from './pages/SinhVien/SinhVienDashboard';
import GiangVienDashboard from './pages/GiangVien/GiangVienDashboard';
import DonViHuongDanDashboard from './pages/DonViHuongDan/DonViHuongDanDashboard';
import AdminDashboard from './pages/Admin/AdminDashboard';

const LOGIN_PAGES = {
  khoa: KhoaLogin,
  giangvien: GiangVienLogin,
  sinhvien: StudentLogin,
  dvhd: DonViHuongDanLogin,
  admin: AdminLogin,
};

function App() {
  const [currentRole, setCurrentRole] = useState('khoa');
  const [session, setSession] = useState(null);
  const CurrentLoginPage = LOGIN_PAGES[currentRole] ?? KhoaLogin;

  function handleRoleChange(nextRole) {
    setCurrentRole(nextRole);
    setSession(null);
  }

  function handleLoginSuccess(account) {
    setSession({
      ...account,
      roleId: currentRole,
    });
  }

  function handleLogout() {
    setSession(null);
  }

  if (!session) {
    return (
      <CurrentLoginPage
        currentRole={currentRole}
        onRoleChange={handleRoleChange}
        onLoginSuccess={handleLoginSuccess}
      />
    );
  }

  if (session.roleId === 'khoa') {
    return <KhoaDashboard account={session} onLogout={handleLogout} />;
  }

  if (session.roleId === 'sinhvien') {
    return <SinhVienDashboard account={session} onLogout={handleLogout} />;
  }

  if (session.roleId === 'giangvien') {
    return <GiangVienDashboard account={session} onLogout={handleLogout} />;
  }

  if (session.roleId === 'dvhd') {
    return <DonViHuongDanDashboard account={session} onLogout={handleLogout} />;
  }

  if (session.roleId === 'admin') {
    return <AdminDashboard account={session} onLogout={handleLogout} />;
  }

  return (
    <main style={{
      minHeight: '100svh',
      display: 'grid',
      placeItems: 'center',
      padding: 24,
      background: '#eef3f2',
      color: '#182026',
    }}>
      <section style={{
        width: 'min(520px, 100%)',
        display: 'grid',
        gap: 14,
        border: '1px solid #d8e1e3',
        borderRadius: 8,
        padding: 24,
        background: '#ffffff',
        boxShadow: '0 16px 42px rgba(24, 32, 38, 0.08)',
      }}>
        <span style={{
          color: '#60717b',
          fontSize: 12,
          fontWeight: 800,
          textTransform: 'uppercase',
        }}>
          Đã đăng nhập
        </span>
        <h1 style={{ margin: 0, fontSize: 28 }}>{session.role}</h1>
        <p style={{ margin: 0, color: '#60717b' }}>
          Màn hình nghiệp vụ cho vai trò này sẽ được nối tiếp theo cùng cấu trúc.
        </p>
        <button
          type="button"
          onClick={handleLogout}
          style={{
            minHeight: 42,
            border: 0,
            borderRadius: 8,
            background: '#157a74',
            color: '#ffffff',
            font: 'inherit',
            fontWeight: 800,
            cursor: 'pointer',
          }}
        >
          Quay lại đăng nhập
        </button>
      </section>
    </main>
  );
}

export default App;
