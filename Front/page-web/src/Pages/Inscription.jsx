import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./../styles/Inscription.css"; 
import ApiService from "./../Services/ApiService";

function Register() {
    const [userName, setUserName] = useState("");
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleRegister = async (e) => {
        e.preventDefault();
        setError("");
        setLoading(true);

        if (password !== confirmPassword) {
            setError("Les mots de passe ne correspondent pas.");
            setLoading(false);
            return;
        }

        try {
            const newUser = {
                userName,
                firstName,
                lastName,
                password,
                admin: false,
            };

            await ApiService.register(newUser);

            navigate("/login");
        } catch (err) {
            setError(err.message || "Une erreur s'est produite.");
        }

        setLoading(false);
    };

    return (
        <div className="register-container">
            <div className="register">
                <h2>S'inscrire</h2>
                {error && <p className="error">{error}</p>}
                <form onSubmit={handleRegister}>
                    <input
                        type="text"
                        placeholder="Nom d'utilisateur"
                        value={userName}
                        onChange={(e) => setUserName(e.target.value)}
                        required />
                    <input
                        type="text"
                        placeholder="Prénom"
                        value={firstName}
                        onChange={(e) => setFirstName(e.target.value)}
                        required />
                    <input
                        type="text"
                        placeholder="Nom"
                        value={lastName}
                        onChange={(e) => setLastName(e.target.value)}
                        required />
                    <input
                        type="password"
                        placeholder="Mot de passe"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required />
                    <input
                        type="password"
                        placeholder="Confirmer le mot de passe"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                        required />
                    <button type="submit" disabled={loading}>
                        {loading ? "Inscription en cours..." : "Créer un compte"}
                    </button>
                </form>
            </div>

            <div className="login-section">
                <p className="login-text">Déjà un compte ?</p>
                <button className="login-button" onClick={() => navigate("/login")}>
                    Se connecter
                </button>
            </div>
        </div>
    );
}

export default Register;
