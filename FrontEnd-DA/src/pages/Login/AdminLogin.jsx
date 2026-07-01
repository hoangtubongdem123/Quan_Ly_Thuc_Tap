import { useState } from 'react';
import LoginRoleSwitcher from '../../components/LoginRoleSwitcher';
import { loginAdmin, logout } from '../../services/authService';
import './AdminLogin.css';

function AdminLogin({ currentRole, onRoleChange, onLoginSuccess }) {
  const [form, setForm] = useState({
    username: '',
    password: '',
  });
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [account, setAccount] = useState(null);
  const [notice, setNotice] = useState(null);

  function updateField(field, value) {
    setForm((current) => ({
      ...current,
      [field]: value,
    }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setLoading(true);
    setNotice(null);

    try {
      const username = form.username.trim();
      const result = await loginAdmin({
        username,
        password: form.password,
      });

      const accountInfo = {
        username: result.taiKhoanAdmin || username,
        idAdmin: result.idAdmin,
        role: 'Admin',
      };

      setAccount(accountInfo);
      onLoginSuccess?.(accountInfo);
      setNotice({
        type: 'success',
        text: result.message || 'Đăng nhập thành công.',
      });
    } catch (error) {
      setAccount(null);
      setNotice({
        type: 'error',
        text: error.message,
      });
    } finally {
      setLoading(false);
    }
  }

  async function handleLogout() {
    setLoading(true);
    setNotice(null);

    try {
      await logout();
    } catch {
      // Clear local UI even when the server session has already expired.
    } finally {
      setAccount(null);
      setForm((current) => ({
        ...current,
        password: '',
      }));
      setNotice({
        type: 'success',
        text: 'Đã đăng xuất khỏi tài khoản admin.',
      });
      setLoading(false);
    }
  }

  return (
    <main className="admin-login-page">
      <section className="admin-aside" aria-label="Không gian quản trị">
        <div className="admin-aside-content">
          <p className="admin-eyebrow">Quản trị hệ thống</p>
          <h1>Điều phối dữ liệu nền cho toàn bộ kỳ thực tập</h1>
          <div className="admin-feature-list">
            <div>
              <strong>01</strong>
              <span>Quản lý sinh viên và giảng viên</span>
            </div>
            <div>
              <strong>02</strong>
              <span>Quản trị khoa, đơn vị và tài khoản</span>
            </div>
            <div>
              <strong>03</strong>
              <span>Theo dõi dữ liệu vận hành hệ thống</span>
            </div>
          </div>
        </div>
      </section>

      <section className="admin-panel" aria-label="Form đăng nhập admin">
        <LoginRoleSwitcher currentRole={currentRole} onRoleChange={onRoleChange} />

        <div className="admin-heading">
          <div className="admin-mark">AD</div>
          <div>
            <p className="admin-eyebrow">Tài khoản Admin</p>
            <h2>{account ? 'Phiên quản trị' : 'Đăng nhập admin'}</h2>
          </div>
        </div>

        <p className="admin-copy">
          Dành cho tài khoản quản trị hệ thống. Vui lòng đăng nhập bằng tài
          khoản admin đã được cấp trong cơ sở dữ liệu.
        </p>

        {account ? (
          <div className="admin-session">
            <div>
              <span>Đang đăng nhập với</span>
              <strong>{account.username}</strong>
              <em>ID Admin: {account.idAdmin || 'N/A'}</em>
            </div>
            <button type="button" className="admin-secondary-button" onClick={handleLogout} disabled={loading}>
              {loading ? 'Đang xử lý...' : 'Đăng xuất'}
            </button>
          </div>
        ) : (
          <form className="admin-login-form" onSubmit={handleSubmit}>
            <label className="admin-field">
              <span>Tài khoản admin</span>
              <input
                autoComplete="username"
                placeholder="admin"
                value={form.username}
                onChange={(event) => updateField('username', event.target.value)}
                required
              />
            </label>

            <label className="admin-field">
              <span>Mật khẩu</span>
              <div className="admin-password-field">
                <input
                  autoComplete="current-password"
                  placeholder="Nhập mật khẩu"
                  type={showPassword ? 'text' : 'password'}
                  value={form.password}
                  onChange={(event) => updateField('password', event.target.value)}
                  required
                />
                <button
                  type="button"
                  onClick={() => setShowPassword((current) => !current)}
                  aria-label={showPassword ? 'Ẩn mật khẩu' : 'Hiện mật khẩu'}
                >
                  {showPassword ? 'Ẩn' : 'Hiện'}
                </button>
              </div>
            </label>

            <button className="admin-primary-button" type="submit" disabled={loading}>
              {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
            </button>
          </form>
        )}

        {notice && (
          <div className={`admin-notice ${notice.type}`} role="status">
            {notice.text}
          </div>
        )}
      </section>
    </main>
  );
}

export default AdminLogin;
