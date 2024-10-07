import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

/*** CSS Imports ***/
import './App.scss';

/*** Package Imports ***/
import { Toaster } from 'react-hot-toast';

/*** Layout Imports ***/
import MainLayout from './layouts/MainLayout/MainLayout';

/*** Page Imports ***/
import Home from './pages/Home/Home';

function App() {
    return (
        <main className='app'>
            <Toaster position='top-right' />
            <Router>
                <Routes>

                    <Route element={<MainLayout />}>
                        <Route path='/' element={<Home />} />
                    </Route>

                </Routes>
            </Router>
        </main>
    );
}

export default App;