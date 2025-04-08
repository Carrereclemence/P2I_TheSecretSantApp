import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import ApiService from "../Services/ApiService";
import PartieApiService from "../Services/ApiServicePartie";
import "./../styles/PageAdmin.css";

function PageAdmin() {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);
  const [users, setUsers] = useState([]);
  const [parties, setParties] = useState([]);
  const [message, setMessage] = useState("");

  useEffect(() => {
    ApiService.getCurrentUser()
      .then((data) => {
        if (!data.admin) {
          navigate("/"); 
        } else {
          setUser(data);
          fetchData();
        }
      })
      .catch(() => navigate("/"));
  }, []);

  const fetchData = async () => {
    try {
      const allUsers = await ApiService.getAllUsers();
      const allParties = await PartieApiService.getAllParties();
      setUsers(allUsers);
      setParties(allParties);
    } catch (err) {
      setMessage("Erreur lors de la récupération des données.");
    }
  };

  const handleDeleteUser = async (id) => {
    if (window.confirm("Supprimer cet utilisateur ?")) {
      try {
        await ApiService.deleteUser(id);
        setMessage("Utilisateur supprimé.");
        fetchData(); // recharge les données
      } catch (err) {
        setMessage("Erreur lors de la suppression.");
      }
    }
  };

  const handleDeletePartie = async (id) => {
    if (window.confirm("Supprimer cette partie ?")) {
      try {
        await PartieApiService.deletePartie(id);
        setMessage("Partie supprimée.");
        fetchData();
      } catch (err) {
        setMessage("Erreur lors de la suppression.");
      }
    }
  };

  return (
    <div className="admin-container">
      <h1>Panneau d'administration 👑</h1>
      {message && <p className="message">{message}</p>}

      <section>
        <h2>Utilisateurs</h2>
        <ul>
          {users.map((u) => (
            <li key={u.id}>
              {u.firstName} {u.lastName} ({u.userName}) -{" "}
              {u.admin ? "Admin" : "Utilisateur"}
              <button onClick={() => handleDeleteUser(u.id)}>🗑</button>
            </li>
          ))}
        </ul>
      </section>

      <section>
        <h2>Parties existantes</h2>
        <ul>
          {parties.map((p) => (
            <li key={p.id}>
              {p.name} - Code: {p.code} - Chef: {p.chef?.userName}
              <button onClick={() => handleDeletePartie(p.id)}>🗑</button>
            </li>
          ))}
        </ul>
      </section>

      <button onClick={() => navigate("/")}>⬅ Retour à l'accueil</button>
    </div>
  );
}

export default PageAdmin;
