import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import ApiService from "./../Services/ApiService";
import PartieApiService from "./../Services/ApiServicePartie";

function PrincipaleCo() {
    const [user, setUser] = useState(null);
    const [partieName, setPartieName] = useState("");
    const [partieCode, setPartieCode] = useState("");
    const [message, setMessage] = useState("");
    const navigate = useNavigate();

    useEffect(() => {
        ApiService.getCurrentUser()
            .then(data => setUser(data))
            .catch(error => console.error("Erreur :", error));
    }, []);

    // 🔹 Fonction pour créer une partie
    const handleCreatePartie = async () => {
        if (!partieName.trim()) {
            setMessage("Veuillez entrer un nom de partie.");
            return;
        }

        try {
            const payload = { name: partieName };
            // ApiService crée la partie et renvoie l'objet créé (incluant l'ID)
            const response = await PartieApiService.createPartie(payload);
            setMessage(`🎉 Partie "${response.name}" créée avec succès !`);
            setPartieName("");
            // Rediriger vers l'URL avec l'ID de la nouvelle partie
            navigate(`/partie/${response.id}`);
        } catch (error) {
            setMessage(error.message);
        }
    };

   // 🔹 Fonction pour rejoindre une partie par code
    const handleJoinPartie = async () => {
        if (!partieCode.trim()) {
            setMessage("Veuillez entrer un code de partie valide.");
            return;
        }

        try {
            // Appel de l'API pour rejoindre la partie en utilisant le code
            const partie = await PartieApiService.joinPartie(partieCode);
            setMessage(`✅ Vous avez rejoint la partie "${partie.name}" !`);
            setPartieCode("");
            // Rediriger vers l'URL paramétrée avec l'ID de la partie
            navigate(`/partie/${partie.id}`);
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
                    <button onClick={() => navigate("/mes-groupes")}>Voir mon groupe Secret Santa</button>

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
                            type="text"
                            placeholder="Code de la partie"
                            value={partieCode}
                            onChange={(e) => setPartieCode(e.target.value)}
                        />
                        <button onClick={handleJoinPartie} disabled={!partieCode.trim()}>
                            Rejoindre
                        </button>
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
