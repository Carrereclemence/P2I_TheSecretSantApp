import React from "react";
import Header from "./Components/Header";
import Principale from "./Pages/Principale-nonco";
import Connection from "./Pages/Connection"
import './App.css'

function App() {
  return (
    <div>
      <Header />
      <main>
        <Principale />
        <Connection />
      </main>
    </div>
  );
}

export default App;