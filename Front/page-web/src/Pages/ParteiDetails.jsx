import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import PartieApiService from "./../Services/ApiServicePartie"; // Assurez-vous du chemin

function PartieDetails() {
  const { id } = useParams();       // Récupère l'ID de la partie dans l'URL
  const navigate = useNavigate();
  const [partie, setPartie] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    PartieApiService.getPartieById(id)
      .then((data) => {
        setPartie(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error("Erreur lors de la récupération de la partie :", err);
        setLoading(false);
      });
  }, [id]);

  if (loading) return <div>Chargement...</div>;
  if (!partie) return <div>Partie introuvable.</div>;

  return (
    <div>
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

      {/* Bouton OK pour revenir à la page principale */}
      <button onClick={() => navigate("/")}>OK</button>
    </div>
  );
}

export default PartieDetails;
