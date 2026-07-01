import { useState } from 'react';
import LoginRoleSwitcher from '../../components/LoginRoleSwitcher';
import { loginSinhVien, logout } from '../../services/authService';
import './StudentLogin.css';

function StudentLogin({ currentRole, onRoleChange, onLoginSuccess }) {
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
      const result = await loginSinhVien({
        email,
        password: form.password,
      });

      const accountInfo = {
        email,
        role: 'Sinh viên',
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
        text: 'Đã đăng xuất khỏi tài khoản sinh viên.',
      });
      setLoading(false);
    }
  }

  return (
    <main className="student-login-page">
      <section className="student-panel" aria-label="Form đăng nhập sinh viên">
        <LoginRoleSwitcher currentRole={currentRole} onRoleChange={onRoleChange} />

        <div className="student-brand">
          <div className="student-mark">SV</div>
          <div>
            <p className="student-eyebrow">TTTN HUCE</p>
            <h1>Đăng nhập sinh viên</h1>
          </div>
        </div>

        <p className="student-intro">
          Theo dõi kỳ thực tập, kết quả đánh giá CLO và các thông báo từ Khoa,
          giảng viên hướng dẫn.
        </p>

        {account ? (
          <div className="student-session">
            <div>
              <span>Đang đăng nhập với</span>
              <strong>{account.email}</strong>
            </div>
            <button type="button" className="student-secondary-button" onClick={handleLogout} disabled={loading}>
              {loading ? 'Đang xử lý...' : 'Đăng xuất'}
            </button>
          </div>
        ) : (
          <form className="student-login-form" onSubmit={handleSubmit}>
            <label className="student-field">
              <span>Email sinh viên</span>
              <input
                autoComplete="username"
                inputMode="email"
                placeholder="mssv@huce.edu.vn"
                value={form.email}
                onChange={(event) => updateField('email', event.target.value)}
                required
              />
            </label>

            <label className="student-field">
              <span>Mật khẩu</span>
              <div className="student-password-field">
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

            <button className="student-primary-button" type="submit" disabled={loading}>
              {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
            </button>
          </form>
        )}

        {notice && (
          <div className={`student-notice ${notice.type}`} role="status">
            {notice.text}
          </div>
        )}
      </section>

      <section className="student-aside" aria-label="Thông tin sinh viên">
        <div className="aside-content">
          <p className="student-eyebrow">Không gian sinh viên</p>
          <h2>Quản lý quá trình thực tập rõ ràng hơn</h2>
          <div className="student-feature-list">
            <div>
              <strong>01</strong>
              <span>Xem trạng thái kỳ thực tập</span>
            </div>
            <div>
              <strong>02</strong>
              <span>Theo dõi điểm CLO</span>
            </div>
            <div>
              <strong>03</strong>
              <span>Nhận thông báo phân công GVHD</span>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}

export default StudentLogin;
