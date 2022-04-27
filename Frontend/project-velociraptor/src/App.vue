<template>
  <div id="app">
    <b-navbar variant='primary' class='navbar py-5'></b-navbar>
    <img alt="project logo" src="./assets/logo.png" class='logo'>
    <h1 class='title'> Project VelociRaptor</h1>
    <Gallary/>
    <b-jumbotron header="CPS643 Virtual Reality" lead="Ryerson University, Department of Computer Science" class='main-desc'>
      <h6>Project Title: Project VelociRaptor</h6>
      <h6>Project Authors: Alex Gomes, Alexander Massin</h6>
      <h6>Project Description: &nbsp;A video game style project with very general flight simulation mechanics taking place in outer space. Galactic enemies
        are here to threaten humanity. Defeat them and survive. There is a shield and health system. When your shield breaks, you have a few seconds
        of invulnerability before you start taking health damage. If you lose all of your health, you lose and must restart. Your task is to defeat
        the boss at the end of the level. Kill enemies along the way to gain powerups such as shield restore, double damage, and invulnerability.
        Each enemy is worth about 100 points and the boss is worth 5000 points. See if you can beat your high score! The project idea is loosely based
        on an iconic old school game <a href="https://store.steampowered.com/app/336060/Raptor_Call_of_The_Shadows__2015_Edition/" target="_blank">Raptor: Call of the Shadows</a>
      </h6>
    </b-jumbotron>
    <div class="embed-responsive embed-responsive-16by9 video-container">
      <iframe class="embed-responsive-item" src="https://www.youtube.com/embed/YfQEnoAPEQA" allowfullscreen></iframe>
    </div>
    <footer class="footer py-5"/>
  </div>
</template>

<script>
import Gallary from './components/Gallary'

export default {
  name: 'App',
  components: {
    Gallary
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

.logo {
  height: 12em;
}

.navbar {
  margin-bottom: 2em;
  background-color: red !important;
}

.footer {
  margin-top: 2em;
  background-color: #3e4acc !important;
}

.title {
  margin: 1em;
}

.main-desc {
  margin-left: 23%;
  margin-right: 23%;
}

.video-container {
  width: 54%;
  margin: auto;
}
</style>
