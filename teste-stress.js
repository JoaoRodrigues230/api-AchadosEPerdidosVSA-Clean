import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
  // Sobe para 100 usuários virtuais simultâneos em 30 segundos
  stages: [
    { duration: '10s', target: 50 },  // Rampa inicial
    { duration: '15s', target: 100 }, // Pico de estresse (100 pessoas logando ao mesmo tempo)
    { duration: '5s', target: 0 },   // Desaceleração
  ],
};

export default function () {
  const url = 'http://localhost:5000/usuario/login';
  
  const payload = JSON.stringify({
    email: 'jv.rodrigues@unisantos.br',
    senha: 'teste123',
  });

  const params = {
    headers: { 'Content-Type': 'application/json' },
  };

  // Dispara a requisição POST de login
  http.post(url, payload, params);
  
  // Aguarda 100ms antes do usuário virtual disparar o próximo login
  sleep(0.1); 
}