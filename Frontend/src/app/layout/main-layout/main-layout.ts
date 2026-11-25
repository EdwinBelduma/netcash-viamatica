
import {
    Component,
    OnInit,
    inject,
    signal
} from '@angular/core';

import {
    Router,
    RouterLink,
    RouterLinkActive,
    RouterOutlet
} from '@angular/router';

import { AuthService } from '../../core/services/auth.service';
import { DashboardService } from '../../core/services/dashboard.service';
import { Menu } from '../../core/models/dashboard.model';

@Component({
    selector: 'app-main-layout',
    standalone: true,
    imports: [
        RouterLink,
        RouterLinkActive,
        RouterOutlet
    ],
    templateUrl: './main-layout.html',
    styleUrl: './main-layout.scss'
})
export class MainLayout implements OnInit {

    private authService = inject(AuthService);
    private dashboardService = inject(DashboardService);
    private router = inject(Router);

    usuario = this.authService.obtenerUsuario();

    menus = signal<Menu[]>([]);
    menuAbierto = signal(false);
    cargandoMenu = signal(true);
    errorMenu = signal('');

    ngOnInit(): void {
        this.cargarMenu();
    }

    cargarMenu(): void {

        this.cargandoMenu.set(true);
        this.errorMenu.set('');

        this.dashboardService.obtenerMenu().subscribe({
            next: respuesta => {
                this.menus.set(respuesta);
                this.cargandoMenu.set(false);
            },
            error: () => {
                this.errorMenu.set(
                    'No se pudo cargar el menú.'
                );
                this.cargandoMenu.set(false);
            }
        });
    }

    obtenerIcono(icono: string): string {

        const iconos: Record<string, string> = {
            'layout-dashboard': 'bi-grid-1x2',
            'users': 'bi-people',
            'contact': 'bi-person-vcard',
            'wallet': 'bi-wallet2',
            'clock': 'bi-clock-history',
            'wifi': 'bi-wifi',
            'router': 'bi-router',
            'file-text': 'bi-file-earmark-text',
            'credit-card': 'bi-credit-card'
        };

        return 'bi ' + (
            iconos[icono] || 'bi-circle'
        );
    }

    alternarMenu(): void {
        this.menuAbierto.update(valor => !valor);
    }

    cerrarMenu(): void {
        this.menuAbierto.set(false);
    }

    cerrarSesion(): void {
        this.authService.cerrarSesion();
        this.router.navigate(['/login']);
    }
}
