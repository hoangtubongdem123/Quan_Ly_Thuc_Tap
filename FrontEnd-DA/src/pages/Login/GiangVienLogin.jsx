import { useState } from 'react';
import LoginRoleSwitcher from '../../components/LoginRoleSwitcher';
import { loginGiangVien, logout } from '../../services/authService';
import './GiangVienLogin.css';

function GiangVienLogin({ currentRole, onRoleChange, onLoginSuccess }) {
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
      const result = await loginGiangVien({
        email,
        password: form.password,
      });

      const accountInfo = {
        email,
        role: 'Giảng viên',
        maSoGiangVien: result.maSoGiangVien,
        tenGiangVien: result.tenGiangVien,
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
        text: 'Đã đăng xuất khỏi tài khoản giảng viên.',
      });
      setLoading(false);
    }
  }

  return (
    <main className="teacher-login-page">
      <section className="teacher-aside" aria-label="Không gian giảng viên">
        <div className="teacher-aside-content">
          <p className="teacher-eyebrow">Cổng giảng viên hướng dẫn</p>
          <h1>Quản lý sinh viên thực tập theo từng chặng</h1>
          <div className="teacher-feature-list">
            <div>
              <strong>01</strong>
              <span>Theo dõi sinh viên phụ trách</span>
            </div>
            <div>
              <strong>02</strong>
              <span>Chấm điểm CLO theo chặng</span>
            </div>
            <div>
              <strong>03</strong>
              <span>Tổng hợp kết quả thực tập</span>
            </div>
          </div>
        </div>
      </section>

      <section className="teacher-panel" aria-label="Form đăng nhập giảng viên">
        <LoginRoleSwitcher currentRole={currentRole} onRoleChange={onRoleChange} />

        <div className="teacher-heading">
          <div className="teacher-mark">GV</div>
          <div>
            <p className="teacher-eyebrow">Tài khoản giảng viên</p>
            <h2>{account ? 'Phiên đăng nhập' : 'Đăng nhập giảng viên'}</h2>
          </div>
        </div>

        <p className="teacher-copy">
          Sử dụng email giảng viên để truy cập khu vực chấm điểm, theo dõi sinh viên
          và xem kết quả đánh giá thực tập.
        </p>

        {account ? (
          <div className="teacher-session">
            <div>
              <span>Đang đăng nhập với</span>
              <strong>{account.tenGiangVien || account.email}</strong>
              <em>{account.maSoGiangVien || account.email}</em>
            </div>
            <button type="button" className="teacher-secondary-button" onClick={handleLogout} disabled={loading}>
              {loading ? 'Đang xử lý...' : 'Đăng xuất'}
            </button>
          </div>
        ) : (
          <form className="teacher-login-form" onSubmit={handleSubmit}>
            <label className="teacher-field">
              <span>Email giảng viên</span>
              <input
                autoComplete="username"
                inputMode="email"
                placeholder="giangvien@huce.edu.vn"
                value={form.email}
                onChange={(event) => updateField('email', event.target.value)}
                required
              />
            </label>

            <label className="teacher-field">
              <span>Mật khẩu</span>
              <div className="teacher-password-field">
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

            <button className="teacher-primary-button" type="submit" disabled={loading}>
              {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
            </button>
          </form>
        )}

        {notice && (
          <div className={`teacher-notice ${notice.type}`} role="status">
            {notice.text}
          </div>
        )}
      </section>
    </main>
  );
}

export default GiangVienLogin;
