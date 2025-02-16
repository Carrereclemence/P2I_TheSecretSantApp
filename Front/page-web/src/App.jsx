import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Header from "./components/Header";
import Principale from "./Pages/Principale-nonco";
import Connection from "./Pages/Connection"
import SeConnecter from "./Pages/SeConnecter"


function App() {
  return (
    <Router>
      <Header />
      <Routes>
        <Route path="/" element={<Principale />} />
        <Route path="/login" element={<Connection />} />
        <Route path="/SeCo" element={<SeConnecter/>} />

      </Routes>
    </Router>
  );
}

export default App;