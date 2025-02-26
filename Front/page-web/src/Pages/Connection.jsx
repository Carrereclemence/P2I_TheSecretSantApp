import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./../styles/Connection.css"; 
import ApiService from "./../Services/ApiService";

function Login() {
    const [userName, setUserName] = useState(""); // UserName au lieu d'email
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false); // Indicateur de chargement
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        setError("");
        setLoading(true);

        try {
                const data = await ApiService.login(userName, password);
                navigate("/");
                window.location.reload(); // 🔥 Recharge la page pour actualiser le state
            } 
        catch (err) {
                setError(err.message);
                setLoading(false);
            }
    };

    return (
        <div className="login">
            <h2>Se Connecter</h2>
            {error && <p className="error">{error}</p>}
            <form onSubmit={handleLogin}>
                <input 
                    type="text" 
                    placeholder="Nom d'utilisateur" 
                    value={userName} 
                    onChange={(e) => setUserName(e.target.value)} 
                    required 
                />
                <input 
                    type="password" 
                    placeholder="Mot de passe" 
                    value={password} 
                    onChange={(e) => setPassword(e.target.value)} 
                    required 
                />
                <button type="submit" disabled={loading}>
                    {loading ? "Connexion..." : "Connexion"}
                </button>
            </form>
        </div>
    );
}

export default Login;
