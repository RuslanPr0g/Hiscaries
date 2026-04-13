import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmptyStateComponent } from './empty-state.component';

describe('EmptyStateComponent', () => {
    let fixture: ComponentFixture<EmptyStateComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [EmptyStateComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EmptyStateComponent);
        fixture.componentRef.setInput('message', 'Nothing here yet.');
        fixture.detectChanges();
    });

    // Requirements 6.1 – icon is rendered
    it('should render the icon', () => {
        fixture.componentRef.setInput('icon', '🎉');
        fixture.detectChanges();

        const icon: HTMLElement = fixture.nativeElement.querySelector('.empty-state__icon');
        expect(icon.textContent?.trim()).toBe('🎉');
    });

    // Requirements 6.2 – default icon is 📚
    it('should use the default icon 📚 when no icon is provided', () => {
        const icon: HTMLElement = fixture.nativeElement.querySelector('.empty-state__icon');
        expect(icon.textContent?.trim()).toBe('📚');
    });

    // Requirements 6.3 – message is rendered
    it('should render the message', () => {
        fixture.componentRef.setInput('message', 'No stories found');
        fixture.detectChanges();

        const message: HTMLElement = fixture.nativeElement.querySelector('.empty-state__message');
        expect(message.textContent?.trim()).toBe('No stories found');
    });

    // Requirements 6.4 – description rendered when non-null
    it('should render the description when it is provided', () => {
        fixture.componentRef.setInput('description', 'Try a different search term.');
        fixture.detectChanges();

        const desc: HTMLElement | null = fixture.nativeElement.querySelector('.empty-state__description');
        expect(desc).not.toBeNull();
        expect(desc!.textContent?.trim()).toBe('Try a different search term.');
    });

    // Requirements 6.5 – description absent when null
    it('should not render the description when it is null', () => {
        fixture.componentRef.setInput('description', null);
        fixture.detectChanges();

        const desc: HTMLElement | null = fixture.nativeElement.querySelector('.empty-state__description');
        expect(desc).toBeNull();
    });
});
