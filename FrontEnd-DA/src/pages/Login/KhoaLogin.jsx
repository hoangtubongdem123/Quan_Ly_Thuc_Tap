import { useState } from 'react';
import LoginRoleSwitcher from '../../components/LoginRoleSwitcher';
import { loginKhoa, logout } from '../../services/authService';
import './KhoaLogin.css';

function KhoaLogin({ currentRole, onRoleChange, onLoginSuccess }) {
  const [form, setForm] = useState({
    email: '',
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
      const email = form.email.trim();
      const result = await loginKhoa({
        email,
        password: form.password,
      });

      const accountInfo = {
        email,
        role: 'Khoa',
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
      // Local state is still cleared when the server session already expired.
    } finally {
      setAccount(null);
      setForm((current) => ({
        ...current,
        password: '',
      }));
      setNotice({
        type: 'success',
        text: 'Đã đăng xuất khỏi tài khoản Khoa.',
      });
      setLoading(false);
    }
  }

  return (
    <main className="login-page">
      <section className="login-hero" aria-labelledby="khoa-login-title">
        <div className="hero-content">
          <p className="eyebrow">Hệ thống thực tập tốt nghiệp</p>
          <h1 id="khoa-login-title">Đăng nhập Khoa / Viện</h1>
          <p className="hero-copy">
            Truy cập khu vực điều phối kỳ thực tập, quản lý danh sách sinh viên
            và phân công giảng viên hướng dẫn.
          </p>

          <div className="hero-stats" aria-label="Chức năng chính">
            <div>
              <strong>01</strong>
              <span>Tạo kỳ thực tập</span>
            </div>
            <div>
              <strong>02</strong>
              <span>Import sinh viên</span>
            </div>
            <div>
              <strong>03</strong>
              <span>Phân công GVHD</span>
            </div>
          </div>
        </div>
      </section>

      <section className="login-panel" aria-label="Form đăng nhập Khoa">
        <LoginRoleSwitcher currentRole={currentRole} onRoleChange={onRoleChange} />

        <div className="panel-heading">
          <div className="panel-mark">K</div>
          <div>
            <p className="eyebrow">Tài khoản Khoa</p>
            <h2>{account ? 'Phiên đăng nhập' : 'Chào mừng trở lại'}</h2>
          </div>
        </div>

        {account ? (
          <div className="signed-in-box">
            <div>
              <span>Đang đăng nhập với</span>
              <strong>{account.email}</strong>
            </div>
            <button type="button" className="secondary-button" onClick={handleLogout} disabled={loading}>
              {loading ? 'Đang xử lý...' : 'Đăng xuất'}
            </button>
          </div>
        ) : (
          <form className="login-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Email Khoa</span>
              <input
                autoComplete="username"
                inputMode="email"
                placeholder="cntt@huce.edu.vn"
                value={form.email}
                onChange={(event) => updateField('email', event.target.value)}
                required
              />
            </label>

            <label className="field">
              <span>Mật khẩu</span>
              <div className="password-field">
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

            <button className="primary-button" type="submit" disabled={loading}>
              {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
            </button>
          </form>
        )}

        {notice && (
          <div className={`notice ${notice.type}`} role="status">
            {notice.text}
          </div>
        )}
      </section>
    </main>
  );
}

export default KhoaLogin;
