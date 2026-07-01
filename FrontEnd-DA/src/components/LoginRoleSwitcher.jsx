import './LoginRoleSwitcher.css';

const LOGIN_ROLES = [
  { id: 'khoa', label: 'Khoa' },
  { id: 'giangvien', label: 'Giảng viên' },
  { id: 'sinhvien', label: 'Sinh viên' },
  { id: 'dvhd', label: 'Đơn vị hướng dẫn' },
  { id: 'admin', label: 'Admin' },
];

function LoginRoleSwitcher({ currentRole, onRoleChange }) {
  return (
    <section className="login-role-switcher" aria-label="Chọn vai trò đăng nhập">
      <span>Đăng nhập với tư cách</span>
      <div className="login-role-options">
        {LOGIN_ROLES.map((role) => (
          <button
            key={role.id}
            type="button"
            className={currentRole === role.id ? 'active' : ''}
            onClick={() => onRoleChange(role.id)}
          >
            {role.label}
          </button>
        ))}
      </div>
    </section>
  );
}

export default LoginRoleSwitcher;
