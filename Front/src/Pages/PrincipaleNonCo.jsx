import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./../styles/PrincipaleNonCo.css"; 

function Principale() {
    const navigate = useNavigate();
  return  (
    <div>
        <h1 className="page-title">The Secret Sant'App 🎁</h1>
        <div className="explication">
            <h2 className="titre-explication">Explication du jeu</h2>
            <p>Le Secret Santa, littéralement “Père Noël secret“ en français, est une tradition qui anime beaucoup de groupes d’amis, de collègues ou de familles durant la période de Noël. Le concept est simple : au sein d’un groupe, chaque participant se voit attribuer une autre personne à qui il doit offrir un cadeau. L’identité de chaque “Santa” (Père Noël) reste secrète jusqu’à l’échange des cadeaux, d’où le nom de l’événement. Le Secret Santa est une excellente façon de célébrer Noël en entreprise, entre amis, ou même en famille. Il permet de renforcer les liens qui unissent les participants, tout en respectant un budget déterminé à l’avance. Le cadeau offert reflète souvent la personnalité ou les goûts de la personne tirée au sort (si elle a joué le jeu…), ajoutant une note personnelle et attentionnée à l’événement.</p>
        </div>
        <div className="connexion-container">
            <h4>Pour continuer, il faut que vous vous connectiez :</h4>
            <div className="connexion">
                <p>J'ai déjà un compte :</p>
                <button onClick={() => navigate("/login")}>Se connecter</button>
            </div>

            <div className="connexion">
                <p>Je n'ai pas de compte :</p>
                <button onClick={() => navigate("/register")}>
                    Créer un compte
                </button>
            </div>
        </div>
    </div>
  );
}

export default Principale;