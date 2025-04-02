import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import ApiService from "../Services/ApiService";
import PartieApiService from "../Services/ApiServicePartie"; 
import "./../styles/PartieDetails.css";  

function PartieDetails() {
  const { id } = useParams();       // Récupère l'ID de la partie dans l'URL
  const navigate = useNavigate();
  const [partie, setPartie] = useState(null);
  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState(null);
  const [destinataire, setDestinataire] = useState(null);


  useEffect(() => {
    const fetchDetails = async () => {
      try {
        const userData = await ApiService.getCurrentUser();
        setUser(userData);

        const partieData = await PartieApiService.getPartieById(id);
        setPartie(partieData);

        // Essaye de récupérer le destinataire
        try {
          const dest = await PartieApiService.getMonDestinataire(id);
          setDestinataire(dest);
        } catch (err) {
          console.log("🎁 Aucun destinataire trouvé ou tirage non effectué :", err.message);
        }

        setLoading(false);
      } catch (err) {
        console.error("Erreur lors de la récupération des infos :", err);
        setLoading(false);
      }
    };

    fetchDetails();
  }, [id]);


  if (loading) return <div>Chargement...</div>;
  if (!partie) return <div>Partie introuvable.</div>;

  const handleTirage = async () => {
    try {
      const response = await fetch(`http://localhost:5286/ApiParties/Parties/${id}/tirage`, {
        method: "POST",
        headers: {
          "Authorization": `Bearer ${localStorage.getItem("token")}`,
          "Content-Type": "application/json"
        }
      });

      const result = await response.json();
      if (!response.ok) throw new Error(result.message || "Erreur pendant le tirage");

      alert("🎉 Tirage effectué avec succès !");
      window.location.reload(); // Recharge pour mettre à jour le destinataire
    } catch (err) {
      alert("❌ Erreur : " + err.message);
    }
  };

  const isChef = user && partie && user.id === partie.chef?.id;

  return (
    <div className="partie-details">
      <h1>Partie : {partie.name}</h1>
      <p><strong>Code :</strong> {partie.code}</p>
      <p><strong>ID :</strong> {partie.id}</p>

      <h3>Chef de la partie</h3>
      {partie.chef ? (
        <p>
          {partie.chef.firstName} {partie.chef.lastName} ({partie.chef.userName})
        </p>
      ) : (
        <p>Aucun chef attribué.</p>
      )}

      <h3>Participants</h3>
      <ul>
        {partie.users && partie.users.length ? (
          partie.users.map((u) => (
            <li key={u.id}>
              {u.firstName} {u.lastName} ({u.userName})
            </li>
          ))
        ) : (
          <li>Aucun participant pour le moment.</li>
        )}
      </ul>

      {isChef && (
        <div className="tirage-section">
          <h3>🔄 Effectuer le tirage Secret Santa</h3>
          <button onClick={handleTirage}>🎁 Tirer au sort</button>
        </div>
      )}


      {destinataire && (
        <div className="destinataire-section">
          <h3>🎁 La personne à qui tu dois offrir un cadeau :</h3>
          <p>
            <strong>{destinataire.firstName} {destinataire.lastName}</strong> ({destinataire.userName})
          </p>
        </div>
      )}

      {/* Bouton OK pour revenir à la page principale */}
      <button onClick={() => navigate("/")}>OK</button>
    </div>
  );
}

export default PartieDetails;
