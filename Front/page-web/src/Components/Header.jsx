import React from "react";
import "./../styles/Header.css"; 
import logo from "./../assets/logo.jpg"; 
import { FaUserCircle } from "react-icons/fa"; 

function Header() {
  return  (
    <header className="header">
      {/* Logo à gauche */}
      <div className="logo-container">
        <img src={logo} alt="Logo Secret Santa" className="logo" />
        <h1>The Secret Sant'App</h1>
      </div>

      {/* Icône de connexion à droite */}
      <div className="login-icon">
        <a href="/login">
          <FaUserCircle size={32} />
        </a>
      </div>
    </header>
  );
}

export default Header;