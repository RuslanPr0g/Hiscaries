import { getJestProjectsAsync } from '@nx/jest';
import type { Config } from 'jest';

export default async (): Promise<Config> => ({
  projects: await getJestProjectsAsync(),
  transform: {
    '^.+\\.(ts|js|mjs)$': [
      'ts-jest',
      {
        useESM: true,
        tsconfig: '<rootDir>/tsconfig.spec.json',
      },
    ],
  },
  transformIgnorePatterns: [
    '/node_modules/(?![@autofiy/autofiyable|@autofiy/property]).+\\.js$',
    '/node_modules/(?![@autofiy/autofiyable|@autofiy/property]).+\\.ts$',
    '/node_modules/(?![@autofiy/autofiyable|@autofiy/property]).+\\.tsx$',
  ],
  moduleFileExtensions: ['ts', 'js', 'mjs', 'json', 'html'],
  testEnvironment: 'jsdom',
  extensionsToTreatAsEsm: ['.ts'],
  globals: {
    'ts-jest': {
      useESM: true,
      tsconfig: '<rootDir>/tsconfig.spec.json',
      isolatedModules: true,
    },
  },
});
