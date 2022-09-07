'use strict';

// Selecting elements
const player0El = document.querySelector('.player--0');
const player1El = document.querySelector('.player--1');
const score0El = document.querySelector('#score--0');
const score1El = document.getElementById('score--1'); //only work for ID
const currentScore0 = document.getElementById('current--0');
const currentScore1 = document.getElementById('current--1');
const diceEl = document.querySelector('.dice');
const btnNew = document.querySelector('.btn--new');
const btnRoll = document.querySelector('.btn--roll');
const btnHold = document.querySelector('.btn--hold');

let scores, currentScore, activePlayer, playing;

// NOTE Creaing a function to start the game
const startGame = function () {
    currentScore = 0;
    activePlayer = 0; // to keep track of active player
    scores = [0, 0];
    playing = true; // state variable to check whether game is still playing

    score0El.textContent = 0;
    score1El.textContent = 0;
    diceEl.classList.add('hidden');
    btnRoll.classList.remove('hidden');
    btnHold.classList.remove('hidden');
    // remove winner player class
    player0El.classList.remove('player--winner');
    player1El.classList.remove('player--winner');
    player0El.classList.add('player--active');
    player1El.classList.remove('player--active');
};
//   document
//     .querySelector(`.player--${activePlayer}`)
//     .classList.toggle('player--winner');
// };

startGame();

//NOTE Creating a fucntion to switch player
const switchPlayer = function () {
    document.getElementById(`current--${activePlayer}`).textContent = 0;
    activePlayer = activePlayer === 0 ? 1 : 0;
    currentScore = 0;
    player0El.classList.toggle('player--active'); // toggle: will remove if exist, will add if do not exist
    player1El.classList.toggle('player--active');
};

// NOTE Add Roll btn functionality
btnRoll.addEventListener('click', function () {
    if (playing) {
        // 1. Generating random dice roll
        const dice = Math.trunc(Math.random() * 6) + 1;

        // 2. Display dice
        diceEl.classList.remove('hidden');
        diceEl.src = `/image/dice-${dice}.png`; // changing the src for the image

        // 3. Check for dice number 1. If true, switch to next player
        if (dice !== 1) {
            // Add dice number to the current score
            currentScore += dice;
            document.getElementById(`current--${activePlayer}`).textContent =
                currentScore;
        } else {
            switchPlayer();
        }
    }
});

// NOTE Add Hold btn functionality
btnHold.addEventListener('click', function () {
    if (playing) {
        // 1. Add current score to active player's total score
        scores[activePlayer] += currentScore;
        document.getElementById(`score--${activePlayer}`).textContent =
            scores[activePlayer];
        // 2. Check if player's score is >= 100, if yes end the game, if not switch player
        if (scores[activePlayer] >= 50) {
            playing = false;
            diceEl.classList.add('hidden');
            btnHold.classList.add('hidden');
            btnRoll.classList.add('hidden');
            document
                .querySelector(`.player--${activePlayer}`)
                .classList.add('player--winner');
            document
                .querySelector(`.player--${activePlayer}`)
                .classList.remove('player--acive');
        } else {
            switchPlayer();
        }
    }
});

// NOTE Add New btn functionality
btnNew.addEventListener('click', startGame);
