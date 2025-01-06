import { useState, useEffect } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'

function App() {
  const [count, setCount] = useState(0)
  const [todos, setTodos] = useState([]);

  useEffect(() => {
    fetch(`${import.meta.env.VITE_API_URL}/application/todo`)
      .then(response => {
        response.json()
          .then(json =>{
            console.log(json);
            setTodos(json);
          });
       
      });
   }, []);

  console.log("TODO List", todos);

  return (
    <>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
      {todos.map(todo => {
        return (
          <div>{todo.title}</div>
        )
      })

      }
    </>
  )
}

export default App;
