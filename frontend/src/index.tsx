import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';

// 建立 root 元素
const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

// 渲染應用程式
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
); 