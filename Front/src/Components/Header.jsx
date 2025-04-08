import React, { useEffect, useState } from "react";
import "./../styles/Header.css"; 
import logo from "./../assets/logo.jpg"; 
import { FaUserCircle } from "react-icons/fa"; 
import { useNavigate } from "react-router-dom";
import ApiService from "./../Services/ApiService";

function Header() {
  const navigate = useNavigate();
  const token = localStorage.getItem("token");
  const [user, setUser] = useState(null);

  useEffect(() => {
    if (token) {
      ApiService.getCurrentUser()
        .then((data) => setUser(data))
        .catch(() => setUser(null));
    }
  }, [token]);

  const handleLogout = () => {
    ApiService.logout();
    navigate("/login");
    window.location.reload();
  };

  return (
    <header className="header">
      <div className="logo-container">
        <button onClick={() => navigate("/")}>
          <img src={logo} alt="Logo Secret Santa" className="logo" />
        </button>
        <h1>The Secret Sant'App</h1>
      </div>

      <div className="login-icon">
        {token && user?.admin && (
          <button onClick={() => navigate("/admin")}>Administrateur</button>
        )}

        {token ? (
          <>
            <button onClick={() => navigate("/profil")}>Mon Profil</button>
            <button onClick={handleLogout}>Se Déconnecter</button>
          </>
        ) : (
          <a href="/login">
            <FaUserCircle size={32} color="white" />
          </a>
        )}
      </div>
    </header>
  );
}

export default Header;
