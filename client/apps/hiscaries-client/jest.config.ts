import type { Config } from 'jest';

const config: Config = {
  preset: 'jest-preset-angular',
  displayName: 'hiscaries-client',
  coverageDirectory: '../../coverage/apps/hiscaries-client',
  setupFilesAfterEnv: ['<rootDir>/src/setup-jest.ts'],
  transformIgnorePatterns: ['node_modules/(?!.*\\.mjs$)'],
  moduleNameMapper: {
    '^@admin/(.*)$': '<rootDir>/src/app/admin/$1',
    '^@environments/(.*)$': '<rootDir>/src/environments/$1',
    '^@shared/(.*)$': '<rootDir>/src/app/shared/$1',
    '^@media/(.*)$': '<rootDir>/src/app/media/$1',
    '^@stories/(.*)$': '<rootDir>/src/app/stories/$1',
    '^@user-to-story/(.*)$': '<rootDir>/src/app/user-to-story/$1',
    '^@users/(.*)$': '<rootDir>/src/app/users/$1',
  },
};

export default config;
