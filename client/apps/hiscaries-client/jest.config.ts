import type { Config } from 'jest';

const config: Config = {
  preset: 'jest-preset-angular',
  displayName: 'hiscaries-client',
  coverageDirectory: '../../coverage/apps/hiscaries-client',
  setupFilesAfterEnv: ['<rootDir>/src/setup-jest.ts'],
  transformIgnorePatterns: ['node_modules/(?!.*\\.mjs$)'],
};

export default config;
