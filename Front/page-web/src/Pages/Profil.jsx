import React, { useEffect, useState } from "react";
import ApiService from "../Services/ApiService";
import "./../styles/Profil.css"; 


function MonProfil() {
  const [user, setUser] = useState(null);
  const [form, setForm] = useState({ firstName: "", lastName: "", password: "" });
  const [message, setMessage] = useState("");

  useEffect(() => {
    ApiService.getCurrentUser()
      .then((data) => {
        setUser(data);
        setForm({ firstName: data.firstName, lastName: data.lastName, password: "" });
      })
      .catch(() => setMessage("Erreur lors de la récupération des données utilisateur."));
  }, []);

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const payload = {
            firstName: form.firstName,
            lastName: form.lastName,
            admin: user.admin,
            };

            if (form.password && form.password.trim() !== "") {
            payload.password = form.password;
            }

            // 🔁 Envoi à l'API
            await ApiService.updateUser(user.id, payload);
            setMessage("Profil mis à jour avec succès !");
            setForm({ ...form, password: "" }); 
        } catch (err) {
            setMessage(err.message);
        }
    };


  if (!user) return <div>Chargement...</div>;

  return (
    <div className="profil-container">
      <div className="profil-box">
        <h1>Mon Profil</h1>
        {message && <p className="message">{message}</p>}

        <form onSubmit={handleSubmit}>
          <label>Prénom :</label>
          <input
            type="text"
            value={form.firstName}
            onChange={(e) => setForm({ ...form, firstName: e.target.value })}
            required
          />

          <label>Nom :</label>
          <input
            type="text"
            value={form.lastName}
            onChange={(e) => setForm({ ...form, lastName: e.target.value })}
            required
          />

          <label>Nouveau mot de passe :</label>
          <input
            type="password"
            value={form.password}
            onChange={(e) => setForm({ ...form, password: e.target.value })}
            placeholder="Laisser vide pour ne pas changer"
          />

          <button type="submit">Enregistrer</button>
        </form>
      </div>
    </div>
  );

}

export default MonProfil;
