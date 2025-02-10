import React from "react";
import "./../styles/Connection.css"; 

function Connection() {
    return (
        <div className="connection">
            <h2>CONNECTION</h2>
            <div className="connexion">
                <p>J'ai déjà un compte :</p>
                <button onClick={() => navigate("/login")}>Se connecter</button>
            </div>

            <div className="connexion">
                <p>Je n'ai pas de compte :</p>
                <button onClick={() => navigate("/register")}>Créer un compte</button>
            </div>
        </div>
    );
}

export default Connection;