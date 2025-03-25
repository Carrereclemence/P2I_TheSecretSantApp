import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import PartieApiService from '../Services/ApiServicePartie';

function MesGroupes() {
  const [parties, setParties] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    PartieApiService.getMyParties()
      .then((data) => {
        setParties(data);
        setLoading(false);
      })
      .catch((error) => {
        setMessage(error.message);
        setLoading(false);
      });
  }, []);

  if (loading) return <div>Chargement...</div>;
  if (!parties || parties.length === 0)
    return <div>{message || "Vous n'êtes dans aucune partie."}</div>;

  return (
    <div>
      <h1>Mes Groupes Secret Santa</h1>
      {parties.map((party) => (
         <div key={party.id} style={{ border: '1px solid #ccc', marginBottom: '10px', padding: '10px' }}>
           <h2>{party.name}</h2>
           <p><strong>Code :</strong> {party.code}</p>
           <p>
             <strong>Chef :</strong>{" "}
             {party.chef ? `${party.chef.firstName} ${party.chef.lastName}` : "Non défini"}
           </p>
           <h3>Participants :</h3>
           <ul>
             {party.users.map((user) => (
               <li key={user.id}>
                 {user.firstName} {user.lastName} ({user.userName})
               </li>
             ))}
           </ul>
           <button onClick={() => navigate(`/partie/${party.id}`)}>
             Voir les détails de la partie
           </button>
         </div>
      ))}
    </div>
  );
}

export default MesGroupes;
