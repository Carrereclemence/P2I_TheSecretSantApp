import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import ApiService from "./../Services/ApiService";

function PrincipaleCo() {
    const [user, setUser] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        ApiService.getCurrentUser()
            .then(data => setUser(data))
            .catch(error => console.error("Erreur :", error));
    }, []);

    return (
        <div>
            <h1>Bienvenue sur The Secret Sant'App 🎁</h1>
            {user ? (
                <div>
                    <h2>Bonjour {user.firstName} {user.lastName} !</h2>
                    <p>Votre pseudo : {user.userName}</p>
                    <p>Statut : {user.admin ? "Administrateur" : "Utilisateur"}</p>
                    <button onClick={() => navigate("/mon-groupe")}>Voir mon groupe Secret Santa</button>
                </div>
            ) : (
                <p>Chargement des informations...</p>
            )}
        </div>
    );
}

export default PrincipaleCo;
