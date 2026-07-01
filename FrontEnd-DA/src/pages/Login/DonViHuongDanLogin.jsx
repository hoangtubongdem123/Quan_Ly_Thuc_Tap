import { useState } from 'react';
import LoginRoleSwitcher from '../../components/LoginRoleSwitcher';
import { loginDonViHuongDan, logout } from '../../services/authService';
import './DonViHuongDanLogin.css';

function DonViHuongDanLogin({ currentRole, onRoleChange, onLoginSuccess }) {
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
      const result = await loginDonViHuongDan({
        email,
        password: form.password,
      });

      const accountInfo = {
        email,
        role: 'Đơn vị hướng dẫn',
        idDonViHD: result.idDonViHD,
        tenDonViHD: result.tenDonViHD,
        idKiThucTap: result.idKiThucTap,
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
        text: 'Đã đăng xuất khỏi tài khoản đơn vị hướng dẫn.',
      });
      setLoading(false);
    }
  }

  return (
    <main className="dvhd-login-page">
      <section className="dvhd-panel" aria-label="Form đăng nhập đơn vị hướng dẫn">
        <LoginRoleSwitcher currentRole={currentRole} onRoleChange={onRoleChange} />

        <div className="dvhd-heading">
          <div className="dvhd-mark">ĐV</div>
          <div>
            <p className="dvhd-eyebrow">Tài khoản ĐVHD</p>
            <h1>{account ? 'Phiên đăng nhập' : 'Đăng nhập đơn vị hướng dẫn'}</h1>
          </div>
        </div>

        <p className="dvhd-copy">
          Truy cập khu vực theo dõi sinh viên tại đơn vị, chấm điểm thực tập và
          quản lý minh chứng theo từng kỳ.
        </p>

        {account ? (
          <div className="dvhd-session">
            <div>
              <span>Đang đăng nhập với</span>
              <strong>{account.tenDonViHD || account.email}</strong>
              <em>ID ĐVHD: {account.idDonViHD || 'N/A'}</em>
              <em>Kỳ thực tập: {account.idKiThucTap || 'N/A'}</em>
            </div>
            <button type="button" className="dvhd-secondary-button" onClick={handleLogout} disabled={loading}>
              {loading ? 'Đang xử lý...' : 'Đăng xuất'}
            </button>
          </div>
        ) : (
          <form className="dvhd-login-form" onSubmit={handleSubmit}>
            <label className="dvhd-field">
              <span>Email đơn vị hướng dẫn</span>
              <input
                autoComplete="username"
                inputMode="email"
                placeholder="donvi@company.com"
                value={form.email}
                onChange={(event) => updateField('email', event.target.value)}
                required
              />
            </label>

            <label className="dvhd-field">
              <span>Mật khẩu</span>
              <div className="dvhd-password-field">
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

            <button className="dvhd-primary-button" type="submit" disabled={loading}>
              {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
            </button>
          </form>
        )}

        {notice && (
          <div className={`dvhd-notice ${notice.type}`} role="status">
            {notice.text}
          </div>
        )}
      </section>

      <section className="dvhd-aside" aria-label="Nghiệp vụ đơn vị hướng dẫn">
        <div className="dvhd-aside-content">
          <p className="dvhd-eyebrow">Không gian doanh nghiệp</p>
          <h2>Đánh giá thực tập rõ ràng, đúng tiến độ</h2>
          <div className="dvhd-feature-list">
            <div>
              <strong>01</strong>
              <span>Theo dõi sinh viên tại đơn vị</span>
            </div>
            <div>
              <strong>02</strong>
              <span>Chấm điểm từng chặng thực tập</span>
            </div>
            <div>
              <strong>03</strong>
              <span>Quản lý minh chứng và phản hồi</span>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}

export default DonViHuongDanLogin;
