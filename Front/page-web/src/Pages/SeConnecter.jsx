import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

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
            const response = await fetch("http://localhost:5286/api/auth/login", {
                method: "POST",
                headers: 
                {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                },
                credentials: "include",
                body: JSON.stringify({ userName, password }), // Utilisation de UserName
            });

            const data = await response.json();
            setLoading(false);

            if (!response.ok) {
                throw new Error(data.message || "Identifiants incorrects");
            }

            localStorage.setItem("token", data.token); // Stockage du token
            navigate("/dashboard"); // Redirection après connexion
        } catch (err) {
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
