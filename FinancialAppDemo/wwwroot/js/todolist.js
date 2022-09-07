// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
'use strict';

const createEl = document.querySelector('#submit');
const closeEl = document.querySelector('.close');
const dateEl = document.querySelector('#date');
const titleEl = document.querySelector('#title');
const taskEl = document.querySelector('#content');
const addListEl = document.querySelector('.right-container');
const changeUser = document.querySelector('.logo');
const listNum = document.querySelectorAll('.list');

// Add functionality for remove button for existing template
const delList = function () {
    addListEl.innerHTML = '';
};

// Add functionality to create new task task
const newList = function () {
    if (!titleEl.value) {
        alert('Please fill in your task!');
    } else {
        // Create new box in right-container
        const createTaskEl = document.createElement('div');
        createTaskEl.classList.add('list', 'box');

        // Create date value in the box
        const createTaskDateEl = document.createElement('p');
        createTaskDateEl.classList.add('date');
        createTaskDateEl.innerText = `Date created: ${dateEl.value}`;

        createTaskEl.appendChild(createTaskDateEl);

        // Create title value in the box
        const createTaskTitleEL = document.createElement('h2');
        createTaskTitleEL.classList.add('title');
        createTaskTitleEL.innerText = titleEl.value;

        createTaskEl.appendChild(createTaskTitleEL);

        // Create content value in the box
        const createTaskContentEl = document.createElement('p');
        createTaskContentEl.classList.add('task');
        createTaskContentEl.innerText = taskEl.value;

        createTaskEl.appendChild(createTaskContentEl);

        // Create button in the box
        const createDelButtonEl = document.createElement('button');
        createDelButtonEl.classList.add('close', 'btn');
        createDelButtonEl.innerText = 'Done';

        createTaskEl.appendChild(createDelButtonEl);

        // Append the whole box in the container
        addListEl.appendChild(createTaskEl);

        // Add functionality for remove button for new box
        createDelButtonEl.addEventListener('click', function () {
            addListEl.removeChild(createTaskEl);
        });
        dateEl.value = '';
        titleEl.value = '';
        taskEl.value = '';
    }
    console.log(listNum);
};

createEl.addEventListener('click', newList);
closeEl.addEventListener('click', delList); // only for the template
document.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') newList();
});
changeUser.addEventListener('click', function () {
    let user = prompt('Please enter your name:');
    if (user === null || user === '') {
        changeUser.innerText = 'Davv';
    } else {
        changeUser.innerText = user;
    }
});

// console.log(listNum);
// To add functionality for 'Submit' btn
// const newList = function (id) {
//   console.log(dateEl.value);
//   console.log(titleEl.value);
//   console.log(taskEl.value);

//   if (!titleEl.value || !taskEl.value) {
//     alert('Please fill in your task!');
//   } else {
//     addListEl.innerHTML += `
//     <div class="list box" id="div_${id}">
//         <p class="date">Date created: ${dateEl.value}</p>
//         <h2 class="title">${titleEl.value}</h2>
//         <p class="task">${taskEl.value}</p>
//         <button class="close btn">&times;</button>
//     </div>
//     `;
//     dateEl.value = '';
//     titleEl.value = '';
//     taskEl.value = '';
//   }
// };
