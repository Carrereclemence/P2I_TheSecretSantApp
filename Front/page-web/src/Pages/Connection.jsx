import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./../styles/Connection.css"; 

function Connection() {
    const navigate = useNavigate();
    return (
        <div className="connection">
            <h2>CONNECTION</h2>
            <div className="connexion">
                <p>J'ai déjà un compte :</p>
                <button onClick={() => navigate("/SeCo")}>Se connecter</button>
            </div>

            <div className="connexion">
                <p>Je n'ai pas de compte :</p>
                <button onClick={() => navigate("/register")}>Créer un compte</button>
            </div>
        </div>
    );
}

export default Connection;