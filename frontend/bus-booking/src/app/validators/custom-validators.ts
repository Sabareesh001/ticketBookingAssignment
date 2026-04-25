import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class CustomValidators {
  /**
   * Validates that source and destination locations are different
   */
  static differentLocations(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const formGroup = control as any;
      if (!formGroup.get) return null;

      const sourceId = formGroup.get('sourceLocationId')?.value;
      const destId = formGroup.get('destinationLocationId')?.value;

      if (sourceId && destId && sourceId === destId) {
        return { sameLocations: true };
      }
      return null;
    };
  }

  /**
   * Validates that a value is a positive number
   */
  static positiveNumber(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      return control.value > 0 ? null : { notPositive: true };
    };
  }

  /**
   * Validates registration number format (e.g., KA-01-AB-1234)
   */
  static registrationNumberFormat(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      const pattern = /^[A-Z]{2}-\d{2}-[A-Z]{2}-\d{4}$/;
      return pattern.test(control.value) ? null : { invalidFormat: true };
    };
  }

  /**
   * Validates postal code (basic - 3-10 alphanumeric characters)
   */
  static postalCode(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      const pattern = /^[a-zA-Z0-9\s\-]{3,10}$/;
      return pattern.test(control.value) ? null : { invalidPostalCode: true };
    };
  }

  /**
   * Validates latitude (-90 to 90)
   */
  static latitude(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value && control.value !== 0) return null;
      const value = parseFloat(control.value);
      return value >= -90 && value <= 90 ? null : { invalidLatitude: true };
    };
  }

  /**
   * Validates longitude (-180 to 180)
   */
  static longitude(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value && control.value !== 0) return null;
      const value = parseFloat(control.value);
      return value >= -180 && value <= 180 ? null : { invalidLongitude: true };
    };
  }
}
