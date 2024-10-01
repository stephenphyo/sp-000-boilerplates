import { Outlet } from 'react-router-dom';

/*** CSS Imports ***/
import './MainLayout.css';

function MainLayout() {
    return (
        <main>
            <section className='app_body'>

                {/* Sidebar */}
                {/* <div className='app_sidebar col-sm-12 col-lg-3'>
                    <Sidebar />
                </div> */}

                {/* Main */}
                <section className='app_main col-sm-12 col-lg-9'>
                    <Outlet />
                </section>
            </section>
        </main>
    );
}

export default MainLayout;