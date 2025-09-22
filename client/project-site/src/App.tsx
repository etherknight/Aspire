import { useState, useEffect } from 'react'
import './App.css'

function App() {
  const [count, setCount] = useState(0)
  const [todos, setTodos] = useState([]);
  const [newTodo, setNewTodo] = useState('');

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

   const handleSubmit = (event: any) => {
      event.preventDefault();
      alert(`The name you entered was: ${newTodo}`)
   };

  console.log("TODO List", todos);

  return (
    <>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        <form onSubmit={handleSubmit}>
          <input type="text"
            value={newTodo}
            onChange={event => setNewTodo(event.target.value)} />
          <button type="submit">Add Todo</button>
        </form>
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
