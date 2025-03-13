import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import ApiService from "./../Services/ApiService";
import PartieApiService from "./../Services/ApiServicePartie";

function PrincipaleCo() {
    const [user, setUser] = useState(null);
    const [partieName, setPartieName] = useState("");
    const [partieId, setPartieId] = useState("");
    const [message, setMessage] = useState("");
    const navigate = useNavigate();

    useEffect(() => {
        ApiService.getCurrentUser()
            .then(data => setUser(data))
            .catch(error => console.error("Erreur :", error));
    }, []);

    // 🔹 Fonction pour créer une partie
    const handleCreatePartie = async () => {
        console.log(user.firstName)
        if (!partieName.trim()) {
            setMessage("Veuillez entrer un nom de partie.");
            return;
        }

        try {
            const response = await PartieApiService.createPartie(partieName);
            setMessage(`🎉 Partie "${response.name}" créée avec succès !`);
            setPartieName("");
        } catch (error) {
            setMessage(error.message);
        }
    };

    // 🔹 Fonction pour empêcher les valeurs négatives ou invalides
    const handlePartieIdChange = (e) => {
        let value = e.target.value;
        
        // Si l'utilisateur efface tout, on autorise la valeur vide
        if (value === "") {
            setPartieId("");
            return;
        }

        // Convertir en nombre
        let numValue = Number(value);

        // Vérifie que ce soit un entier positif
        if (!isNaN(numValue) && Number.isInteger(numValue) && numValue >= 0) {
            setPartieId(value);
        }
    };

    // 🔹 Fonction pour rejoindre une partie
    const handleJoinPartie = async () => {
        if (!partieId.trim() || isNaN(Number(partieId)) || Number(partieId) < 0) {
            setMessage("Veuillez entrer un ID de partie valide.");
            return;
        }

        try {
            await PartieApiService.joinPartie(partieId);
            setMessage(`✅ Vous avez rejoint la partie #${partieId} !`);
            setPartieId("");
            navigate(`/partie/${partieId}`); // Redirige vers la page de la partie
        } catch (error) {
            setMessage(error.message);
        }
    };

    return (
        <div>
            <h1>Bienvenue sur The Secret Sant'App 🎁</h1>
            {user ? (
                <div>
                    <h2>Bonjour {user.firstName} {user.lastName} !</h2>
                    <p>Votre pseudo : {user.userName}</p>
                    <p>Statut : {user.admin ? "Administrateur" : "Utilisateur"}</p>
                    <button onClick={() => navigate("/mon-groupe")}>Voir mon groupe Secret Santa</button>

                    {/* 🔹 Ajout des boutons de gestion des parties */}
                    <div className="gestion-partie">
                        <h3>Créer une Partie</h3>
                        <input
                            type="text"
                            placeholder="Nom de la partie"
                            value={partieName}
                            onChange={(e) => setPartieName(e.target.value)}
                        />
                        <button onClick={handleCreatePartie}>Créer</button>
                    </div>

                    <div className="gestion-partie">
                        <h3>Rejoindre une Partie</h3>
                        <input
                            type="number"
                            placeholder="ID de la partie"
                            value={partieId}
                            onChange={handlePartieIdChange}
                            min="0"
                            step="1"
                        />
                        <button onClick={handleJoinPartie} disabled={partieId === ""}>Rejoindre</button>
                    </div>

                    {/* 🔹 Affichage des messages d'action */}
                    {message && <p className="message">{message}</p>}
                </div>
            ) : (
                <p>Informations indisponibles...</p>
            )}
        </div>
    );
}

export default PrincipaleCo;
