# Space Invaders
<img width="890" height="491" alt="Game preview" src="https://github.com/user-attachments/assets/f6fe0473-d09a-4a77-a427-8bcd0b27963b" />

# 📌 Trabalho Prático – Space Invaders em C# com Windows Forms

Desenvolva uma aplicação desktop em C# utilizando Windows Forms Application (.NET) que reproduza uma versão simplificada do clássico jogo Space Invaders, aplicando conceitos de Programação Orientada a Objetos (POO).

## 🎮 Requisitos obrigatórios:

Controle da Nave
O jogador deverá controlar uma nave localizada na parte inferior da tela utilizando:

Teclas ← e → ou A e D para movimentação horizontal;
A tecla Espaço para disparar projéteis.
Inimigos
O jogo deverá conter uma formação inicial de naves alienígenas organizada em uma matriz de, no mínimo:

3 linhas × 5 colunas (15 aliens).

Os aliens deverão se deslocar pela tela e avançar em direção ao jogador.

Disparos dos Aliens
Os aliens deverão realizar disparos de forma aleatória;
Caso um projétil alienígena atinja a nave do jogador, o jogador perde 1 vida;
O jogador deverá iniciar a partida com 3 vidas.

Condições de Vitória e Derrota
### ✅ Vitória:

O jogador vence quando todos os aliens forem destruídos.

### ❌ Derrota:

O jogador perde quando suas vidas chegarem a zero;
O jogador perde imediatamente caso qualquer alien alcance a parte inferior da janela (borda inferior do formulário).

Colisões
Quando um projétil do jogador atingir um alien, o alien deverá ser removido da tela;
O projétil também deverá desaparecer após a colisão;
Os disparos dos aliens deverão desaparecer ao atingir a nave ou sair da área do jogo.

## 💻 O projeto deve utilizar conceitos de Programação Orientada a Objetos, como:

Classes e objetos;
Encapsulamento;
Construtores;
Associação entre classes;
Métodos e atributos.

Sugestão de classes:

NaveJogador;
Alien;
Projetil;
Jogo;
GerenciadorDeColisoes.

### ⚠️ Observação importante:
> A implementação deve ser inteiramente funcional. Não serão aceitas soluções em que os elementos estejam apenas desenhados na tela sem interação, colisões ou lógica de jogo adequadas.

## 📄 Entrega:

A entrega deverá ser realizada até a próxima quarta-feira, dia 10, às 23h59. 
Entregar:

O projeto completo da aplicação;
Um relatório em PDF contendo:

   
Explicação das classes desenvolvidas;
Descrição da lógica do jogo;
Explicação do controle de movimentação e disparo;
Capturas de tela da aplicação funcionando.

## ✅ Critérios de avaliação:

Funcionamento correto do jogo;
Uso adequado de Programação Orientada a Objetos;
Organização e clareza do código;
Implementação das colisões;
Interface gráfica;
Qualidade do relatório.
